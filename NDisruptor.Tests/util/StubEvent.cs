namespace NDisruptor.Tests
{
    internal class StubEvent
    {
        public static IEventFactory<StubEvent> EVENT_FACTORY = new StubEventFactory();
        private int _value;

        public StubEvent(int i)
        {
            _value = i;
        }

        public void copy(StubEvent expectedEvent)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class StubEventFactory : IEventFactory<StubEvent>
    {
        public StubEvent newInstance()
        {
            return new StubEvent(-1);
        }
    }
}