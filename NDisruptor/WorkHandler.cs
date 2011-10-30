namespace NDisruptor
{
    public interface WorkHandler<T>
    {
        void onEvent(T @event);
    }
}