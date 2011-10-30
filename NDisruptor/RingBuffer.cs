using System;

namespace NDisruptor
{

    public sealed class RingBuffer<T> : Sequencer
    {
        private readonly int indexMask;
        private readonly Object[] entries;

        public RingBuffer(IEventFactory<T> eventFactory,
                          int bufferSize,
                          ClaimStrategy.Option claimStrategyOption,
                          WaitStrategy.Option waitStrategyOption) :
            base(bufferSize, claimStrategyOption, waitStrategyOption)
        {

            if (bufferSize.BitCount() != 1)
            {
                throw new ArgumentException("bufferSize must be a power of 2");
            }

            indexMask = getBufferSize() - 1;
            entries = new Object[getBufferSize()];

            fill(eventFactory);
        }

        public RingBuffer(IEventFactory<T> eventFactory, int size)
            : this(eventFactory, size,
                ClaimStrategy.Option.MULTI_THREADED,
                WaitStrategy.Option.BLOCKING)
        {
        }

        private int getBufferSize()
        {
            throw new NotImplementedException();
        }

        public T get(long sequence)
        {
            return (T)entries[(int)sequence & indexMask];
        }

        private void fill(IEventFactory<T> eventFactory)
        {
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i] = eventFactory.newInstance();
            }
        }
    }
}
