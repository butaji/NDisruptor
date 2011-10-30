using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace NDisruptor.Tests
{
    public class RingBufferTest
    {
        private RingBuffer<StubEvent> ringBuffer;
        private ISequenceBarrier sequenceBarrier;
        private ExecutorService EXECUTOR;

        [SetUp]
        void setup()
        {
            ringBuffer = new RingBuffer<StubEvent>(StubEvent.EVENT_FACTORY, 32);
            ringBuffer.setGatingSequences(new NoOpEventProcessor(ringBuffer).getSequence());
            sequenceBarrier = ringBuffer.newBarrier();
            EXECUTOR = Executors.newSingleThreadExecutor(new DaemonThreadFactory());
        }

        private void assertEquals(object obj1, object obj2)
        {
            Assert.AreEqual(obj1, obj2);
        }

        [Test]
        public void shouldClaimAndGet()
        {
            assertEquals(Sequencer.INITIAL_CURSOR_VALUE, ringBuffer.getCursor());

            StubEvent expectedEvent = new StubEvent(2701);

            long claimSequence = ringBuffer.next();
            assertEquals(0, claimSequence);


            StubEvent oldEvent = ringBuffer.get(claimSequence);
            oldEvent.copy(expectedEvent);
            ringBuffer.publish(claimSequence);

            long sequence = sequenceBarrier.waitFor(0);
            assertEquals(0, sequence);

            StubEvent @event = ringBuffer.get(sequence);
            assertEquals(expectedEvent, @event);

            assertEquals(0L, ringBuffer.getCursor());
        }

        [Test]
        public void shouldClaimAndGetWithTimeout()
        {
            assertEquals(Sequencer.INITIAL_CURSOR_VALUE, ringBuffer.getCursor());

            StubEvent expectedEvent = new StubEvent(2701);

            long claimSequence = ringBuffer.next();
            StubEvent oldEvent = ringBuffer.get(claimSequence);
            oldEvent.copy(expectedEvent);
            ringBuffer.publish(claimSequence);

            long sequence = sequenceBarrier.waitFor(0, TimeSpan.FromMilliseconds(5));
            assertEquals(0, sequence);

            StubEvent @event = ringBuffer.get(sequence);
            assertEquals(expectedEvent, @event);

            assertEquals(0L, ringBuffer.getCursor());
        }

        [Test]
        public void shouldGetWithTimeout()
        {
            long sequence = sequenceBarrier.waitFor(0, TimeSpan.FromMilliseconds(5));
            assertEquals(Sequencer.INITIAL_CURSOR_VALUE, sequence);
        }

        [Test]
        public void shouldClaimAndGetInSeparateThread()
        {
            Future<List<StubEvent>> messages = getMessages(0, 0);

            StubEvent expectedEvent = new StubEvent(2701);

            long sequence = ringBuffer.next();
            StubEvent oldEvent = ringBuffer.get(sequence);
            oldEvent.copy(expectedEvent);
            ringBuffer.publish(sequence);

            assertEquals(expectedEvent, messages.get()[0]);
        }

        [Test]
        public void shouldClaimAndGetMultipleMessages()
        {
            int numMessages = ringBuffer.getBufferSize();
            for (int i = 0; i < numMessages; i++)
            {
                long sequence = ringBuffer.next();
                StubEvent @event = ringBuffer.get(sequence);
                @event.setValue(i);
                ringBuffer.publish(sequence);
            }

            int expectedSequence = numMessages - 1;
            long available = sequenceBarrier.waitFor(expectedSequence);
            assertEquals(expectedSequence, available);

            for (int i = 0; i < numMessages; i++)
            {
                assertEquals(i, ringBuffer.get(i).getValue());
            }
        }

        [Test]
        public void shouldWrap()
        {
            int numMessages = ringBuffer.getBufferSize();
            int offset = 1000;
            for (int i = 0; i < numMessages + offset; i++)
            {
                long sequence = ringBuffer.next();
                StubEvent @event = ringBuffer.get(sequence);
                @event.setValue(i);
                ringBuffer.publish(sequence);
            }

            int expectedSequence = numMessages + offset - 1;
            long available = sequenceBarrier.waitFor(expectedSequence);
            assertEquals(expectedSequence, available);

            for (int i = offset; i < numMessages + offset; i++)
            {
                assertEquals(i, ringBuffer.get(i).getValue());
            }
        }

        [Test]
        public void shouldSetAtSpecificSequence()
        {
            const long expectedSequence = 5;

            ringBuffer.claim(expectedSequence);
            StubEvent expectedEvent = ringBuffer.get(expectedSequence);
            expectedEvent.setValue((int)expectedSequence);
            ringBuffer.forcePublish(expectedSequence);

            long sequence = sequenceBarrier.waitFor(expectedSequence);
            assertEquals(expectedSequence, sequence);

            StubEvent @event = ringBuffer.get(sequence);
            assertEquals(expectedEvent, @event);

            assertEquals(expectedSequence, ringBuffer.getCursor());
        }

        [Test]
        public void shouldPreventPublishersOvertakingEventProcessorWrapPoint()
        {
            int ringBufferSize = 4;
            CountDownLatch latch = new CountDownLatch(ringBufferSize);
            AtomicBoolean publisherComplete = new AtomicBoolean(false);
            RingBuffer<StubEvent> ringBuffer = new RingBuffer<StubEvent>(StubEvent.EVENT_FACTORY, ringBufferSize);
            TestEventProcessor processor = new TestEventProcessor(ringBuffer.newBarrier());
            ringBuffer.setGatingSequences(processor.getSequence());

            Thread thread = new Thread(() =>
            {
                {
                    for (int i = 0; i <= ringBufferSize; i++)
                    {
                        long sequence = ringBuffer.next();
                        StubEvent @event = ringBuffer.get(sequence);
                        @event.setValue(i);
                        ringBuffer.publish(sequence);
                        latch.countDown();
                    }

                    publisherComplete.set(true);
                }
            });

            thread.Start();

            latch.await();
            assertEquals(ringBuffer.getCursor(), ringBufferSize - 1);
            Assert.IsFalse(publisherComplete.get());

            processor.run();
            thread.Join();

            Assert.IsTrue(publisherComplete.get());
        }



        private Future<List<StubEvent>> getMessages(long initial, long toWaitFor)
        {
            CyclicBarrier cyclicBarrier = new CyclicBarrier(2);
            ISequenceBarrier sequenceBarrier = ringBuffer.newBarrier();

            Future<List<StubEvent>> f = EXECUTOR.submit(new TestWaiter(cyclicBarrier, sequenceBarrier, ringBuffer, initial, toWaitFor));

            cyclicBarrier.await();

            return f;
        }

    }
}