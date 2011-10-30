namespace NDisruptor
{
    public class WaitStrategy
    {
        public class Option
        {
            public static object BLOCKING;

            public WaitStrategy newInstance()
            {
                throw new System.NotImplementedException();
            }
        }

        public void signalAllWhenBlocking()
        {
            throw new System.NotImplementedException();
        }

        public long waitFor(long sequence, Sequence cursorSequence, Sequence[] dependentSequences, ProcessingSequenceBarrier processingSequenceBarrier)
        {
            throw new System.NotImplementedException();
        }

        public long waitFor(long sequence, Sequence cursorSequence, Sequence[] dependentSequences, ProcessingSequenceBarrier processingSequenceBarrier, long timeout, TimeUnit units)
        {
            throw new System.NotImplementedException();
        }
    }
}