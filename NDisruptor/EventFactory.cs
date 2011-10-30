namespace NDisruptor
{
    public interface EventFactory<T>
    {
        T newInstance();
    }
}