using System;
using System.Threading;

namespace NDisruptor
{
    class BlockingStrategy : WaitStrategy
    {
        private object _lock = new object();
        private volatile int numWaiters = 0;

        public override long waitFor(long sequence, Sequence cursor, Sequence[] dependents, ISequenceBarrier barrier)
        {
            long availableSequence;
            if ((availableSequence = cursor.get()) < sequence)
            {
                lock (_lock)
                {

                    try
                    {
                        ++numWaiters;
                        while ((availableSequence = cursor.get()) < sequence)
                        {
                            barrier.checkAlert();
                            Monitor.Wait(_lock);
                        }
                    }
                    finally
                    {
                        --numWaiters;
                    }
                }
            }

            if (0 != dependents.Length)
            {
                while ((availableSequence = Util.getMinimumSequence(dependents)) < sequence)
                {
                    barrier.checkAlert();
                }
            }

            return availableSequence;
        }

        public override long waitFor(long sequence, Sequence cursor, Sequence[] dependents, ISequenceBarrier barrier, TimeSpan timeout)
        {
            long availableSequence;
            if ((availableSequence = cursor.get()) < sequence)
            {
                lock (_lock)
                {

                    try
                    {
                        ++numWaiters;
                        while ((availableSequence = cursor.get()) < sequence)
                        {
                            barrier.checkAlert();

                            if (!Monitor.Wait(_lock, timeout))
                            {
                                break;
                            }
                        }
                    }
                    finally
                    {
                        --numWaiters;
                    }
                }
            }

            if (0 != dependents.Length)
            {
                while ((availableSequence = Util.getMinimumSequence(dependents)) < sequence)
                {
                    barrier.checkAlert();
                }
            }

            return availableSequence;
        }

        public override void signalAllWhenBlocking()
        {
            if (0 != numWaiters)
            {
                lock (_lock)
                {
                    Monitor.PulseAll(_lock);
                }
            }
        }
    }


}