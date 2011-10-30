using System;
using System.Collections.Generic;
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

            assertEquals(expectedEvent, messages.get().get(0));
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