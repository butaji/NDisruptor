using System;

namespace NDisruptor
{
    internal class AtomicLongArray
    {
        private long[] _value;

        public AtomicLongArray(int bufferSize)
        {
            _value = new long[bufferSize];
        }

        public int length()
        {
            return _value.Length;
        }

        public void lazySet(int i, long initialCursorValue)
        {
            set(i, initialCursorValue);
        }

        private void lazySetCallBack(IAsyncResult ar)
        {
            if (!ar.IsCompleted)
                throw new ArgumentException();
        }

        public void set(int i, long sequence)
        {
            _value[i] = sequence;
        }

        public decimal get(int i)
        {
            return _value[i];
        }
    }
}