using System;

namespace NDisruptor
{
    public class WorkProcessor : IEventProcessor
    {
        public virtual Sequence getSequence()
        {
            throw new System.NotImplementedException();
        }

        public virtual void halt()
        {
            throw new System.NotImplementedException();
        }

        public virtual void run()
        {
            throw new NotImplementedException();
        }
    }

   public sealed class WorkProcessor<T> : WorkProcessor
{
    private readonly AtomicBoolean running = new AtomicBoolean(false);
    private readonly  Sequence sequence = new Sequence(Sequencer.INITIAL_CURSOR_VALUE);
    private readonly  RingBuffer<T> ringBuffer;
    private readonly  ISequenceBarrier sequenceBarrier;
    private readonly  IWorkHandler<T> workHandler;
    private readonly  IExceptionHandler exceptionHandler;
    private readonly  AtomicLong workSequence;

    public WorkProcessor(RingBuffer<T> ringBuffer,
                         ISequenceBarrier sequenceBarrier,
                         IWorkHandler<T> workHandler,
                         IExceptionHandler exceptionHandler,
                         AtomicLong workSequence)
    {
        this.ringBuffer = ringBuffer;
        this.sequenceBarrier = sequenceBarrier;
        this.workHandler = workHandler;
        this.exceptionHandler = exceptionHandler;
        this.workSequence = workSequence;
    }

    public override Sequence getSequence()
    {
        return sequence;
    }

    public override void halt()
    {
        running.set(false);
        sequenceBarrier.alert();
    }

    public override void run()
    {
        if (!running.compareAndSet(false, true))
        {
            throw new IllegalStateException("Thread is already running");
        }
        sequenceBarrier.clearAlert();

        if (typeof(ILifecycleAware).IsAssignableFrom(workHandler.GetType()))
        {
            ((ILifecycleAware)workHandler).onStart();
        }

        bool processedSequence = true;
        long nextSequence = sequence.get();
        T @event = default(T);
        while (true)
        {
            try
            {
                if (processedSequence)
                {
                    processedSequence = false;
                    nextSequence = workSequence.incrementAndGet();
                    sequence.set(nextSequence - 1L);
                }

                sequenceBarrier.waitFor(nextSequence);
                @event = ringBuffer.get(nextSequence);
                workHandler.onEvent(@event);

                processedSequence = true;
            }
            catch (AlertException ex)
            {
                if (!running.get())
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                exceptionHandler.handle(ex, nextSequence, @event);
                processedSequence = true;
            }
        }

        if (typeof(ILifecycleAware).IsAssignableFrom(workHandler.GetType()))
        {
            ((ILifecycleAware)workHandler).onShutdown();
        }

        running.set(false);
    }
}
}