namespace NDisruptor.Tests
{
    public class DaemonThreadFactory
    {
        public ExecutorService Create()
        {
            return new ExecutorService();
        }
    }
}