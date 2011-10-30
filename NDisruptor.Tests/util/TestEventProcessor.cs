using System;

namespace NDisruptor.Tests
{
    public class TestEventProcessor : IEventProcessor
    {
        private readonly ISequenceBarrier sequenceBarrier;
        private Sequence sequence = new Sequence(Sequencer.INITIAL_CURSOR_VALUE);

        public TestEventProcessor(ISequenceBarrier newBarrier)
        {
            sequenceBarrier = newBarrier;
        }

        public Sequence getSequence()
        {
            return sequence;
        }

        public void halt()
        {
            throw new NotImplementedException();
        }

        public void run()
        {
            try
            {
                sequenceBarrier.waitFor(0L);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            sequence.set(sequence.get() + 1L);
        }
    }
}