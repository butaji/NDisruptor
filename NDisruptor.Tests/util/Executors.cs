namespace NDisruptor.Tests
{
    public class Executors
    {
        public static ExecutorService newSingleThreadExecutor(DaemonThreadFactory daemonThreadFactory)
        {
            return daemonThreadFactory.Create();
        }
    }
}