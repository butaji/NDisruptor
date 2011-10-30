using System;

namespace NDisruptor.Tests
{
    public class CountDownLatch
    {
        private readonly int _ringBufferSize;

        public CountDownLatch(int ringBufferSize)
        {
            _ringBufferSize = ringBufferSize;
        }

        public void countDown()
        {
            throw new NotImplementedException();
        }

        public void await()
        {
            throw new NotImplementedException();
        }
    }
}