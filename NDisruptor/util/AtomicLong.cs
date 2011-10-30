namespace NDisruptor
{
    public class AtomicLong
    {
        private long _value;

        protected AtomicLong(long value)
        {
            _value = value;
        }

        public long incrementAndGet()
        {
            return _value++;
        }

        public void set(long cursor)
        {
            _value = cursor;
        }

        public long get()
        {
            return _value;
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