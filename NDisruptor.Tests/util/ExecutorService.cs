using System.Collections.Generic;

namespace NDisruptor.Tests
{
    public class ExecutorService : Executor
    {
        public Future<List<StubEvent>> submit(TestWaiter testWaiter)
        {
            return testWaiter.StubEventsList();
        }
    }
}