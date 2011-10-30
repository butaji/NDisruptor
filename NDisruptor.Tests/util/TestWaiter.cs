using System;
using System.Collections.Generic;

namespace NDisruptor.Tests
{
    public class TestWaiter
    {
        private CyclicBarrier cyclicBarrier;
        private long initialSequence;
        private RingBuffer<StubEvent> ringBuffer;
        private long toWaitForSequence;
        private ISequenceBarrier sequenceBarrier;

        public TestWaiter(CyclicBarrier cyclicBarrier,
                      ISequenceBarrier sequenceBarrier,
                       RingBuffer<StubEvent> ringBuffer,
                       long initialSequence,
                       long toWaitForSequence)
        {
            this.cyclicBarrier = cyclicBarrier;
            this.initialSequence = initialSequence;
            this.ringBuffer = ringBuffer;
            this.toWaitForSequence = toWaitForSequence;
            this.sequenceBarrier = sequenceBarrier;
        }

        public Future<List<StubEvent>> StubEventsList()
        {
            cyclicBarrier.await();
            sequenceBarrier.waitFor(toWaitForSequence);

            List<StubEvent> messages = new List<StubEvent>();
            for (long l = initialSequence; l <= toWaitForSequence; l++)
            {
                messages.Add(ringBuffer.get(l));
            }

            return new Future<List<StubEvent>>(messages);
        }
    }
}