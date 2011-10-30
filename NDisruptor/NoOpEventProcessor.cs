namespace NDisruptor
{
    public sealed class NoOpEventProcessor : EventProcessor
    {
        private readonly SequencerFollowingSequence sequence;

        /**
         * Construct a {@link EventProcessor} that simply tracks a {@link Sequencer}.
         *
         * @param sequencer to track.
         */
        public NoOpEventProcessor(Sequencer sequencer)
        {
            sequence = new SequencerFollowingSequence(sequencer);
        }

        public Sequence getSequence()
        {
            return sequence;
        }

        public void halt()
        {
        }

        public void run()
        {
        }

        private class SequencerFollowingSequence : Sequence
        {
            private readonly Sequencer sequencer;

            public SequencerFollowingSequence(Sequencer sequencer)
                : base(Sequencer.INITIAL_CURSOR_VALUE)
            {
                this.sequencer = sequencer;
            }

            public override long get()
            {
                return sequencer.getCursor();
            }
        }
    }
}