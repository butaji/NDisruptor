using NUnit.Framework;

namespace NDisruptor.Tests
{
    public class EventTranslatorTest
    {
        private static readonly string TEST_VALUE = "Wibble";

        [Test]
        public void shouldTranslateOtherDataIntoAnEvent()
        {
            StubEvent @event = StubEvent.EVENT_FACTORY.newInstance();
            IEventTranslator<StubEvent> eventTranslator = new ExampleEventTranslator(TEST_VALUE);

            @event = eventTranslator.translateTo(@event, 0);

            Assert.AreEqual(TEST_VALUE, @event.getTestString());
        }

        public class ExampleEventTranslator : IEventTranslator<StubEvent>
        {
            private string testValue;

            public ExampleEventTranslator(string testValue)
            {
                this.testValue = testValue;
            }

            public StubEvent translateTo(StubEvent @event, long sequence)
            {
                @event.setTestString(testValue);
                return @event;
            }
        }
    }
}