using System;

namespace NDisruptor
{
    public abstract class WaitStrategy
    {
        public class Option
        {
            public static Option BLOCKING = new Option();

            public WaitStrategy newInstance()
            {
                return new BlockingStrategy();
            }
        }

        public abstract void signalAllWhenBlocking();
        public abstract long waitFor(long sequence, Sequence cursor, Sequence[] dependents, ISequenceBarrier barrier);

        public abstract long waitFor(long sequence, Sequence cursor, Sequence[] dependents, ISequenceBarrier barrier,
                                     TimeSpan timeout);
    }
}