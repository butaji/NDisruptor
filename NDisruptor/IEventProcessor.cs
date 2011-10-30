namespace NDisruptor
{
    public interface IEventProcessor : IRunnable
    {
        Sequence getSequence();
    }

}