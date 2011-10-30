using System;

namespace NDisruptor
{
    public class Sequencer<T>
    {
        protected Sequencer(int bufferSize, ClaimStrategy.Option claimStrategyOption, WaitStrategy.Option waitStrategyOption)
        {
            throw new NotImplementedException();
        }

        protected Sequencer(EventFactory<T> eventFactory, int claimStrategyOption, object waitStrategyOption, object blocking)
        {
            throw new NotImplementedException();
        }
    }
}