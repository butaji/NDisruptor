namespace NDisruptor
{
    public interface IWorkHandler<T>
    {
        void onEvent(T @event);
    }
}