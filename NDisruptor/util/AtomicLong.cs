using System;
using System.Threading;

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
            for (; ; )
            {
                long current = get();
                long next = current + 1;
                if (compareAndSet(current, next))
                    return next;
            }
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
            //TODO:
            set(value);
        }
    }
}