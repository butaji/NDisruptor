using NUnit.Framework;

namespace NDisruptor.Tests
{
    public class RingBufferTest
    {
        private RingBuffer<StubEvent> ringBuffer;
        private ISequenceBarrier sequenceBarrier;

        [SetUp]
        void setup()
        {
            ringBuffer = new RingBuffer<StubEvent>(StubEvent.EVENT_FACTORY, 32);
            ringBuffer.setGatingSequences(new NoOpEventProcessor(ringBuffer).getSequence());
            sequenceBarrier = ringBuffer.newBarrier();
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

        private void assertEquals(object obj1, object obj2)
        {
            Assert.AreEqual(obj1, obj2);
        }
    }
}