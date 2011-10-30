namespace NDisruptor
{
    public interface EventHandler<T>
    {
        void onEvent(T @event, long sequence, bool endOfBatch);
    }
}