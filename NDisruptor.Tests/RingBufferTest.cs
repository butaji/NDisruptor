using NUnit.Framework;

namespace NDisruptor.Tests
{
    public class RingBufferTest
    {
        private RingBuffer<StubEvent> ringBuffer;
        //private SequenceBarrier sequenceBarrier;

        [SetUp]
        void setup()
        {
            ringBuffer = new RingBuffer<StubEvent>(StubEvent.EVENT_FACTORY, 32);
        }

        [Test]
        public void shouldClaimAndGet()
        {
            Assert.AreEqual(Sequencer.INITIAL_CURSOR_VALUE, ringBuffer.getCursor());

            //StubEvent expectedEvent = new StubEvent(2701);

            //long claimSequence = ringBuffer.next();
            //StubEvent oldEvent = ringBuffer.get(claimSequence);
            //oldEvent.copy(expectedEvent);
            //ringBuffer.publish(claimSequence);

            //long sequence = sequenceBarrier.waitFor(0);
            //Assert.Equals(0, sequence);

            //StubEvent @event = ringBuffer.get(sequence);
            //Assert.Equals(expectedEvent, @event);

            //Assert.Equals(0L, ringBuffer.getCursor());
        }
    }
}