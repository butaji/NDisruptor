namespace NDisruptor
{
    public interface SequenceBarrier
    {
        long waitFor(long sequence);

        long waitFor(long sequence, long timeout, TimeUnit units);

        long getCursor();

        bool isAlerted();

        void alert();

        void clearAlert();

        void checkAlert();
    }

}