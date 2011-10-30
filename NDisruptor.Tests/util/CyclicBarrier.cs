using System;
using System.Threading;

namespace NDisruptor.Tests
{
    public class CyclicBarrier
    {
        private readonly int _value;

        public CyclicBarrier(int value)
        {
            _value = value;
        }

        public void await()
        {
            throw new NotImplementedException();
        }
    }
}