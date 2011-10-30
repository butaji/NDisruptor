namespace NDisruptor
{
    public class ClaimStrategy
    {
        public class Option
        {
            public static object MULTI_THREADED;

            public ClaimStrategy newInstance(int bufferSize)
            {
                return new MultiThreadedStrategy(bufferSize);
            }
        }

        public virtual bool hasAvailableCapacity(Sequence[] gatingSequences)
        {
            throw new System.NotImplementedException();
        }

        public virtual long incrementAndGet(Sequence[] gatingSequences)
        {
            throw new System.NotImplementedException();
        }

        public virtual long incrementAndGet(int gatingSequences, Sequence[] sequences)
        {
            throw new System.NotImplementedException();
        }

        public virtual void setSequence(long sequence, Sequence[] gatingSequences)
        {
            throw new System.NotImplementedException();
        }

        public virtual void serialisePublishing(long sequence, Sequence cursor, int batchSize)
        {
            throw new System.NotImplementedException();
        }
    }

    public class SingleThreadedStrategy
        : ClaimStrategy
    {
        private int bufferSize;
        private PaddedLong minGatingSequence = new PaddedLong(Sequencer.INITIAL_CURSOR_VALUE);
        private readonly PaddedLong claimSequence = new PaddedLong(Sequencer.INITIAL_CURSOR_VALUE);

        public SingleThreadedStrategy(int bufferSize)
        {
            this.bufferSize = bufferSize;
        }

        
        public override bool hasAvailableCapacity(Sequence[] dependentSequences)
        {
            long wrapPoint = (claimSequence.get() + 1L) - bufferSize;
            if (wrapPoint > minGatingSequence.get())
            {
                long minSequence = getMinimumSequence(dependentSequences);
                minGatingSequence.set(minSequence);

                if (wrapPoint > minSequence)
                {
                    return false;
                }
            }

            return true;
        }

        private long getMinimumSequence(Sequence[] sequences)
        {
            throw new System.NotImplementedException();
        }

        public override long incrementAndGet(Sequence[] dependentSequences)
        {
            long nextSequence = claimSequence.get() + 1L;
            claimSequence.set(nextSequence);
            waitForFreeSlotAt(nextSequence, dependentSequences);

            return nextSequence;
        }

        public override long incrementAndGet(int delta, Sequence[] dependentSequences)
        {
            long nextSequence = claimSequence.get() + delta;
            claimSequence.set(nextSequence);
            waitForFreeSlotAt(nextSequence, dependentSequences);

            return nextSequence;
        }

        public override void setSequence(long sequence, Sequence[] dependentSequences)
        {
            claimSequence.set(sequence);
            waitForFreeSlotAt(sequence, dependentSequences);
        }

        public override void serialisePublishing(long sequence, Sequence cursor, int batchSize)
        {
            cursor.set(sequence);
        }

        private void waitForFreeSlotAt(long sequence, Sequence[] dependentSequences)
        {
            long wrapPoint = sequence - bufferSize;
            if (wrapPoint > minGatingSequence.get())
            {
                long minSequence;
                while (wrapPoint > (minSequence = getMinimumSequence(dependentSequences)))
                {
                    LockSupport.parkNanos(1000L);
                }

                minGatingSequence.set(minSequence);
            }
        }
    }
}