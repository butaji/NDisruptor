using NUnit.Framework;

namespace NDisruptor.Tests
{
    public class EventPublisherTest : IEventTranslator<LongEvent>
    {
        [Test]
        public void shouldPublishEvent()
        {
            var ringBuffer = new RingBuffer<LongEvent>(LongEvent.FACTORY, 32);
            ringBuffer.setGatingSequences(new NoOpEventProcessor(ringBuffer).getSequence());
            var eventPublisher = new EventPublisher<LongEvent>(ringBuffer);

            eventPublisher.publishEvent(this);
            eventPublisher.publishEvent(this);

            Assert.AreEqual(ringBuffer.get(0).get(), 0 + 29L);
            Assert.AreEqual(ringBuffer.get(1).get(), 1 + 29L);
        }

        public LongEvent translateTo(LongEvent @event, long sequence)
        {
            @event.set(sequence + 29);
            return @event;
        }

    }

    public class LongEvent : IEventFactory<LongEvent>
    {
        public static IEventFactory<LongEvent> FACTORY = new LongEvent();
        private long _value;

        public long get()
        {
            return _value;
        }

        public void set(long l)
        {
            _value = l;
        }

        public LongEvent newInstance()
        {
            return new LongEvent();
        }
    }
}