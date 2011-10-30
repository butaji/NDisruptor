using System;

namespace NDisruptor
{
    public sealed class BatchEventProcessor<T>
        : EventProcessor
    {
        private AtomicBoolean running = new AtomicBoolean(false);
        private ExceptionHandler exceptionHandler = new FatalExceptionHandler();
        private RingBuffer<T> ringBuffer;
        private SequenceBarrier sequenceBarrier;
        private NDisruptor.EventHandler<T> eventHandler;
        private readonly Sequence sequence = new Sequence(Sequencer.INITIAL_CURSOR_VALUE);

        public BatchEventProcessor(RingBuffer<T> ringBuffer,
                                   SequenceBarrier sequenceBarrier,
                                   EventHandler<T> eventHandler)
        {
            this.ringBuffer = ringBuffer;
            this.sequenceBarrier = sequenceBarrier;
            this.eventHandler = eventHandler;

            if (typeof (SequenceReportingEventHandler<T>).IsAssignableFrom(eventHandler.GetType()))
            {
                ((SequenceReportingEventHandler<T>) eventHandler).setSequenceCallback(sequence);
            }
        }

        public Sequence getSequence()
        {
            return sequence;
        }

        public void halt()
        {
            running.set(false);
            sequenceBarrier.alert();
        }

        public void setExceptionHandler(ExceptionHandler exceptionHandler)
        {
            if (null == exceptionHandler)
            {
                throw new NullReferenceException();
            }

            this.exceptionHandler = exceptionHandler;
        }

        public void run()
        {
            if (!running.compareAndSet(false, true))
            {
                throw new IllegalStateException("Thread is already running");
            }
            sequenceBarrier.clearAlert();

            if (typeof(LifecycleAware).
            IsAssignableFrom(eventHandler.GetType()))
            {
                ((LifecycleAware) eventHandler).onStart();
            }

            T @event = default(T);
            long nextSequence = sequence.get() + 1L;
            while (true)
            {
                try
                {
                    long availableSequence = sequenceBarrier.waitFor(nextSequence);
                    while (nextSequence <= availableSequence)
                    {
                    @event = ringBuffer.get(nextSequence);
                        eventHandler.onEvent(@event,
                        nextSequence,
                        nextSequence == availableSequence)
                        ;
                        nextSequence++;
                    }

                    sequence.set(nextSequence - 1L);
                }
                catch (AlertException ex)
                {
                    if (!running.get())
                    {
                        break;
                    }
                }
            catch
                (Exception ex)
                {
                    exceptionHandler.handle(ex, nextSequence,  @event)
                    ;
                    sequence.set(nextSequence);
                    nextSequence++;
                }
            }

            if (typeof(LifecycleAware).IsAssignableFrom(eventHandler.GetType()))
            {
                ((LifecycleAware) eventHandler).onShutdown();
            }

            running.set(false);
        }
    }
}