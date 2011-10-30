namespace NDisruptor
{
    public interface IEventProcessor : IRunnable
    {
        Sequence getSequence();

        void halt();
    }

}