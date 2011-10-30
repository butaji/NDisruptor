using System;

namespace NDisruptor
{
 public class Sequencer
 {
    public static readonly long INITIAL_CURSOR_VALUE = -1L;

     protected readonly int bufferSize;

    private readonly Sequence cursor = new Sequence(INITIAL_CURSOR_VALUE);
    private Sequence[] gatingSequences;

    private readonly ClaimStrategy claimStrategy;
    private readonly WaitStrategy waitStrategy;

    public Sequencer(int bufferSize,
                     ClaimStrategy.Option claimStrategyOption,
                     WaitStrategy.Option waitStrategyOption)
    {
        this.claimStrategy = claimStrategyOption.newInstance(bufferSize);
        this.waitStrategy = waitStrategyOption.newInstance();
        this.bufferSize = bufferSize;
    }

     public void setGatingSequences(params Sequence[] sequences)
    {
        this.gatingSequences = sequences;
    }

    public ISequenceBarrier newBarrier(params Sequence[] sequencesToTrack)
    {
        return new ProcessingSequenceBarrier(waitStrategy, cursor, sequencesToTrack);
    }

    public BatchDescriptor newBatchDescriptor(int size)
    {
        return new BatchDescriptor(Math.Min(size, bufferSize));
    }

    public int getBufferSize()
    {
        return bufferSize;
    }

    public long getCursor()
    {
        return cursor.get();
    }

    public bool hasAvailableCapacity()
    {
        return claimStrategy.hasAvailableCapacity(gatingSequences);
    }

    public long next()
    {
        if (null == gatingSequences)
        {
            throw new NullReferenceException("gatingSequences must be set before claiming sequences");
        }

        return claimStrategy.incrementAndGet(gatingSequences);
    }

    public BatchDescriptor next(BatchDescriptor batchDescriptor)
    {
        if (null == gatingSequences)
        {
            throw new NullReferenceException("gatingSequences must be set before claiming sequences");
        }

        long sequence = claimStrategy.incrementAndGet(batchDescriptor.getSize(), gatingSequences);
        batchDescriptor.setEnd(sequence);
        return batchDescriptor;
    }

    public long claim(long sequence)
    {
        if (null == gatingSequences)
        {
            throw new NullReferenceException("gatingSequences must be set before claiming sequences");
        }

        claimStrategy.setSequence(sequence, gatingSequences);

        return sequence;
    }

    public void publish(long sequence)
    {
        publish(sequence, 1);
    }


    public void publish(BatchDescriptor batchDescriptor)
    {
        publish(batchDescriptor.getEnd(), batchDescriptor.getSize());
    }

    public void forcePublish(long sequence)
    {
        cursor.set(sequence);
        waitStrategy.signalAllWhenBlocking();
    }

    private void publish(long sequence, int batchSize)
    {
        claimStrategy.serialisePublishing(sequence, cursor, batchSize);
        waitStrategy.signalAllWhenBlocking();
    }
}
}