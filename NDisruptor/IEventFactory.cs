namespace NDisruptor
{
    public interface IEventFactory<T>
    {
        T newInstance();
    }
}