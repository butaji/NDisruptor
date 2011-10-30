namespace NDisruptor
{
    public interface IEventHandler<T>
    {
        void onEvent(T @event, long sequence, bool endOfBatch);
    }
}