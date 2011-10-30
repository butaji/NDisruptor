using System;

namespace NDisruptor
{
    public interface ISequenceBarrier
    {
        long waitFor(long sequence);

        long waitFor(long sequence, TimeSpan units);

        long getCursor();

        bool isAlerted();

        void alert();

        void clearAlert();

        void checkAlert();
    }

}