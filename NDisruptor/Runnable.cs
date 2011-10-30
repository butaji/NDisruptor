namespace NDisruptor
{
    public interface Runnable
    {
        Sequence getSequence();

        void halt();

        void run();
    }
}