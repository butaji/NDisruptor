using System.Threading;

namespace NDisruptor
{
public sealed class WorkerPool<T>
{
    private readonly AtomicBoolean started = new AtomicBoolean(false);
    private readonly PaddedAtomicLong workSequence = new PaddedAtomicLong(Sequencer.INITIAL_CURSOR_VALUE);
    private readonly RingBuffer<T> ringBuffer;
    private readonly WorkProcessor[] workProcessors;

    public WorkerPool(RingBuffer<T> ringBuffer,
                      ISequenceBarrier sequenceBarrier,
                      IExceptionHandler exceptionHandler,
                      params IWorkHandler<T>[] workHandlers)
    {
        this.ringBuffer = ringBuffer;
        int numWorkers = workHandlers.Length;
        workProcessors = new WorkProcessor[numWorkers];

        for (int i = 0; i < numWorkers; i++)
        {
            workProcessors[i] = new WorkProcessor<T>(ringBuffer,
                                                     sequenceBarrier,
                                                     workHandlers[i],
                                                     exceptionHandler,
                                                     workSequence);
        }
    }

    public WorkerPool(IEventFactory<T> eventFactory,
                      int size,
                      ClaimStrategy.Option claimStrategyOption,
                      WaitStrategy.Option waitStrategyOption,
                      IExceptionHandler exceptionHandler,
                      params IWorkHandler<T>[] workHandlers)
    {
        ringBuffer = new RingBuffer<T>(eventFactory, size, claimStrategyOption, waitStrategyOption);
        ISequenceBarrier barrier = ringBuffer.newBarrier();
        int numWorkers = workHandlers.Length;
        workProcessors = new WorkProcessor[numWorkers];

        for (int i = 0; i < numWorkers; i++)
        {
            workProcessors[i] = new WorkProcessor<T>(ringBuffer,
                                                     barrier,
                                                     workHandlers[i],
                                                     exceptionHandler,
                                                     workSequence);
        }

        ringBuffer.setGatingSequences(getWorkerSequences());
    }

    public Sequence[] getWorkerSequences()
    {
        Sequence[] sequences = new Sequence[workProcessors.Length];
        for (int i = 0, size = workProcessors.Length; i < size; i++)
        {
            sequences[i] = workProcessors[i].getSequence();
        }

        return sequences;
    }

    public RingBuffer<T> start(Executor executor)
    {
        if (!started.compareAndSet(false, true))
        {
            throw new IllegalStateException("WorkerPool has already been started and cannot be restarted until halted.");
        }

        long cursor = ringBuffer.getCursor();
        workSequence.set(cursor);

        foreach (WorkProcessor processor in workProcessors)
        {
            processor.getSequence().set(cursor);
            executor.execute(processor);
        }

        return ringBuffer;
    }

    public void drainAndHalt()
    {
        Sequence[] workerSequences = getWorkerSequences();
        while (ringBuffer.getCursor() > Util.getMinimumSequence(workerSequences))
        {
            Thread.Yield();
        }

        foreach (WorkProcessor processor in workProcessors)
        {
            processor.halt();
        }

        started.set(false);
    }

    public void halt()
    {
        foreach (WorkProcessor processor in workProcessors)
        {
            processor.halt();
        }

        started.set(false);
    }
}
}