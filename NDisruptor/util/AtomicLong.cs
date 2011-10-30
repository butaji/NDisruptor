using System;
using System.Threading;

namespace NDisruptor
{
    public class AtomicLong
    {
        private readonly Action<long> _lazySet;
        private long _value;

        protected AtomicLong(long value)
        {
            _lazySet = v => set(value);
            _value = value;
        }

        public long incrementAndGet()
        {
            return _value++;
        }

        public long set(long cursor)
        {
            return _value = cursor;
        }

        public long get()
        {
            return _value;
        }

        public bool compareAndSet(long expect, long update)
        {
            var r = Interlocked.CompareExchange(ref _value, update, expect);
            return _value == update;
        }

        public long addAndGet(int delta)
        {
            throw new System.NotImplementedException();
        }


        public void lazySet(long value)
        {
            //misc.unsafe, inline_unsafe_ordered_store
            _lazySet.BeginInvoke(value, lazySetCallback, this);
        }

        private void lazySetCallback(IAsyncResult ar)
        {
            if (!ar.IsCompleted)
                throw new ArgumentException();
        }
    }
}