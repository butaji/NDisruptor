namespace NDisruptor
{
    public interface EventTranslator<T>
    {
        T translateTo(T @event, long sequence);
    }
}