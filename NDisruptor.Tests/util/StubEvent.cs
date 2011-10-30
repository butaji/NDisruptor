using System;

namespace NDisruptor.Tests
{
    public class StubEvent
    {
        public static IEventFactory<StubEvent> EVENT_FACTORY = new StubEventFactory();
        private int _value;

        public StubEvent(int i)
        {
            _value = i;
        }

        public void copy(StubEvent expectedEvent)
        {
            _value = expectedEvent._value;
        }

        public bool Equals(StubEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._value == _value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (StubEvent)) return false;
            return Equals((StubEvent) obj);
        }

        public override int GetHashCode()
        {
            return _value;
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