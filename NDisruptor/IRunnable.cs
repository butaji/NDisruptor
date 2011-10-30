namespace NDisruptor
{
    public interface IRunnable
    {
        Sequence getSequence();

        void halt();

        void run();
    }
}