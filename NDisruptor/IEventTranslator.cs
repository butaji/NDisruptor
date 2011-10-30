namespace NDisruptor
{
    public interface IEventTranslator<T>
    {
        T translateTo(T @event, long sequence);
    }
}