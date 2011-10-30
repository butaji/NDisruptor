namespace NDisruptor
{
    public class EventPublisher<E>
    {
        private readonly RingBuffer<E> ringBuffer;

        public EventPublisher(RingBuffer<E> ringBuffer)
        {
            this.ringBuffer = ringBuffer;
        }

        public void publishEvent(EventTranslator<E> translator)
        {
            long sequence = ringBuffer.next();
            try
            {
                translator.translateTo(ringBuffer.get(sequence), sequence);
            }
            finally
            {
                ringBuffer.publish(sequence);
            }
        }
    }
}