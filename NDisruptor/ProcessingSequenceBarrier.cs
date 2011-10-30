namespace NDisruptor
{
    public class ProcessingSequenceBarrier : ISequenceBarrier
{
    private readonly WaitStrategy waitStrategy;
    private readonly Sequence cursorSequence;
    private readonly Sequence[] dependentSequences;
    private volatile bool alerted = false;

    public ProcessingSequenceBarrier(WaitStrategy waitStrategy,
                                     Sequence cursorSequence,
                                     Sequence[] dependentSequences)
    {
        this.waitStrategy = waitStrategy;
        this.cursorSequence = cursorSequence;
        this.dependentSequences = dependentSequences;
    }

    public long waitFor(long sequence)
    {
        checkAlert();

        return waitStrategy.waitFor(sequence, cursorSequence, dependentSequences, this);
    }

    public long waitFor(long sequence, long timeout, TimeUnit units)
    {
        checkAlert();

        return waitStrategy.waitFor(sequence, cursorSequence, dependentSequences, this, timeout, units);
    }

    public long getCursor()
    {
        return cursorSequence.get();
    }

    public bool isAlerted()
    {
        return alerted;
    }

    public void alert()
    {
        alerted = true;
        waitStrategy.signalAllWhenBlocking();
    }

    public void clearAlert()
    {
        alerted = false;
    }

    public void checkAlert() 
    {
        if (alerted)
        {
            throw new AlertException();
        }
    }
}
}