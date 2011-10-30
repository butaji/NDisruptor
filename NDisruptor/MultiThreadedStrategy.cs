using System;

namespace NDisruptor
{
    public sealed class MultiThreadedStrategy
        : ClaimStrategy
    {
        private readonly int bufferSize;
        private readonly int indexMask;
        private readonly AtomicLongArray pendingPublications;
        private readonly PaddedAtomicLong claimSequence = new PaddedAtomicLong(Sequencer.INITIAL_CURSOR_VALUE);
        private readonly PaddedAtomicLong csLock = new PaddedAtomicLong(0L);

        private readonly OverridedThreadLocal<MutableLong> minGatingSequenceThreadLocal = new OverridedThreadLocal<MutableLong>()
                                                                                              {
          
                                                                                              };

        public MultiThreadedStrategy(int bufferSize)
        {
            if (bufferSize.BitCount() != 1)
            {
                throw new ArgumentException("bufferSize must be a power of 2");
            }

            this.bufferSize = bufferSize;
            indexMask = bufferSize - 1;
            pendingPublications = new AtomicLongArray(bufferSize);
            for (int i = 0, size = pendingPublications.length(); i < size; i++)
            {
                pendingPublications.lazySet(i, Sequencer.INITIAL_CURSOR_VALUE);
            }
        }

        public override bool hasAvailableCapacity(Sequence[] dependentSequences)
        {
            MutableLong minGatingSequence = minGatingSequenceThreadLocal.get();
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

        public override  long incrementAndGet(Sequence[] dependentSequences)
        {
            MutableLong minGatingSequence = minGatingSequenceThreadLocal.get();
            waitForCapacity(dependentSequences, minGatingSequence);

            long nextSequence = claimSequence.incrementAndGet();
            waitForFreeSlotAt(nextSequence, dependentSequences, minGatingSequence);

            return nextSequence;
        }

        public override long incrementAndGet(int delta, Sequence[] dependentSequences)
    {
        long nextSequence = claimSequence.addAndGet(delta);
        waitForFreeSlotAt(nextSequence, dependentSequences, minGatingSequenceThreadLocal.get());

        return nextSequence;
    }

        public override void setSequence(long sequence, Sequence[] dependentSequences)
    {
        claimSequence.lazySet(sequence);
        waitForFreeSlotAt(sequence, dependentSequences, minGatingSequenceThreadLocal.get());
    }

        public override void serialisePublishing(long sequence, Sequence cursor, int batchSize)
    {
        long expectedSequence = sequence - batchSize;
        if (expectedSequence == cursor.get())
        {
            cursor.set(sequence);
            if (sequence == claimSequence.get())
            {
                return;
            }
        }
        else
        {
            for (long i = expectedSequence + 1; i < sequence; i++)
            {
                pendingPublications.lazySet((int)i & indexMask, i);
            }

            pendingPublications.set((int)sequence & indexMask, sequence);
        }

        if (csLock.compareAndSet(0L, 1L))
        {
            long initialCursor = cursor.get();
            long currentCursor = initialCursor;

            while (currentCursor < claimSequence.get())
            {
                long nextSequence = currentCursor + 1L;
                if (nextSequence != pendingPublications.get((int)nextSequence & indexMask))
                {
                    break;
                }

                currentCursor = nextSequence;
            }

            if (currentCursor > initialCursor)
            {
                cursor.set(currentCursor);
            }

            csLock.set(0L);
        }
    }

        private void waitForCapacity(Sequence[] dependentSequences,  MutableLong minGatingSequence)
    {
        long wrapPoint = (claimSequence.get() + 1L) - bufferSize;
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

        private void waitForFreeSlotAt(long sequence, Sequence[] dependentSequences, MutableLong minGatingSequence)
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

        private long getMinimumSequence(Sequence[] dependentSequences)
        {
            throw new NotImplementedException();
        }
    }
}