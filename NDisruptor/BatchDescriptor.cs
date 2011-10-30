namespace NDisruptor
{
    public sealed class BatchDescriptor
    {
        private readonly int size;
        private long end = Sequencer.INITIAL_CURSOR_VALUE;

        public BatchDescriptor(int size)
        {
            this.size = size;
        }

        public long getEnd()
        {
            return end;
        }

        public void setEnd(long end)
        {
            this.end = end;
        }

        public int getSize()
        {
            return size;
        }

        public long getStart()
        {
            return end - (size - 1L);
        }
    }
}