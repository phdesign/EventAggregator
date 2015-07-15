using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EventAggregator.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ObserverShouldOnlyReceiveSubscribedNotifications()
        {
            // Arrange

            var eventAggregator = new EventAggregator();
            var observer1 = new Mock<Action<TestEvent>>();
            var observer2 = new Mock<Action<OtherEvent>>();

            eventAggregator.Subscribe(observer1.Object);
            eventAggregator.Subscribe(observer2.Object);
            
            var testEvent = new TestEvent();
            var otherEvent = new OtherEvent();

            // Act

            eventAggregator.Publish(testEvent);
            eventAggregator.Publish(otherEvent);

            // Assert

            observer1.Verify(x => x(testEvent), Times.Once);
            observer2.Verify(x => x(otherEvent), Times.Once);
        }

        [TestMethod]
        public void ObserverStopsReceivingEventsWhenUnsubscribed()
        {
            // Arrange

            var eventAggregator = new EventAggregator();
            var observer1 = new Mock<Action<TestEvent>>();

            var unsubscribe = eventAggregator.Subscribe(observer1.Object);
            var testEvent1 = new TestEvent();
            var testEvent2 = new TestEvent();

            // Act

            eventAggregator.Publish(testEvent1);
            unsubscribe.Dispose();
            eventAggregator.Publish(testEvent2);

            // Assert

            observer1.Verify(x => x(testEvent1), Times.Once);
            observer1.Verify(x => x(testEvent2), Times.Never);
        }

        [TestMethod]
        public void BaseObserverReceivesAllEvents()
        {
            // Arrange

            var eventAggregator = new EventAggregator();
            var observer1 = new Mock<Action<IEvent>>();
            var observer2 = new Mock<Action<TestEvent>>();

            eventAggregator.Subscribe(observer1.Object);
            eventAggregator.Subscribe(observer2.Object);

            var testEvent = new TestEvent();
            var derivedEvent = new DerivedEvent();

            // Act

            eventAggregator.Publish(testEvent);
            eventAggregator.Publish(derivedEvent);

            // Assert

            observer1.Verify(x => x(It.IsAny<IEvent>()), Times.Exactly(2));
            observer2.Verify(x => x(It.IsAny<TestEvent>()), Times.Exactly(2));
        }

        public class TestEvent : IEvent { }
        public class OtherEvent : IEvent { }
        public class DerivedEvent : TestEvent { }

        public class TestEventHandler : IObserve<TestEvent>
        {
            public void Subscribe(IEventAggregator aggregator)
            {
                aggregator.Subscribe<TestEvent>(Handle);
            }

            public void Handle(TestEvent message)
            {
                throw new NotImplementedException();
            }
        }
    }
}
