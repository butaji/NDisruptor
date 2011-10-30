namespace NDisruptor.Tests
{
    internal class StubEvent
    {
        public static IEventFactory<StubEvent> EVENT_FACTORY;

        public StubEvent(int i)
        {
            throw new System.NotImplementedException();
        }

        public void copy(StubEvent expectedEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}