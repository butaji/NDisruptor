namespace NDisruptor
{
    public class PaddedAtomicLong : AtomicLong
    {
        public long p1, p2, p3, p4, p5, p6 = 7L;

        public PaddedAtomicLong(long value)
            : base(value)
        {
        }

        public long sumPaddingToPreventOptimisation()
        {
            return p1 + p2 + p3 + p4 + p5 + p6;
        }

        public bool compareAndSet(long l, long l1)
        {
            throw new System.NotImplementedException();
        }

        public long addAndGet(int delta)
        {
            throw new System.NotImplementedException();
        }


        public void lazySet(long value)
        {
            throw new System.NotImplementedException();
        }
   
    }

}