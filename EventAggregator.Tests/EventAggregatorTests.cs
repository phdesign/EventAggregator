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
            var observer1 = new Mock<IObserve<TestEvent>>();
            var observer2 = new Mock<IObserve<OtherEvent>>();

            eventAggregator.Subscribe(observer1.Object);
            eventAggregator.Subscribe(observer2.Object);
            
            var testEvent = new TestEvent();
            var otherEvent = new OtherEvent();

            // Act

            eventAggregator.Publish(testEvent);
            eventAggregator.Publish(otherEvent);

            // Assert

            observer1.Verify(x => x.OnNext(It.IsAny<IEvent>()), Times.Once);
            observer1.Verify(x => x.OnNext(testEvent), Times.Once);
            observer2.Verify(x => x.OnNext(otherEvent), Times.Once);
        }

        [TestMethod]
        public void ObserverStopsReceivingEventsWhenUnsubscribed()
        {
            // Arrange

            var eventAggregator = new EventAggregator();
            var observer1 = new Mock<IObserve<TestEvent>>();

            var unsubscribe = eventAggregator.Subscribe(observer1.Object);
            var testEvent1 = new TestEvent();
            var testEvent2 = new TestEvent();

            // Act

            eventAggregator.Publish(testEvent1);
            unsubscribe.Dispose();
            eventAggregator.Publish(testEvent2);

            // Assert

            observer1.Verify(x => x.OnNext(It.IsAny<IEvent>()), Times.Once);
            observer1.Verify(x => x.OnNext(testEvent1), Times.Once);
            observer1.Verify(x => x.OnNext(testEvent2), Times.Never);
        }

        [TestMethod]
        public void BaseObserverReceivesAllEvents()
        {
            // Arrange

            var eventAggregator = new EventAggregator();
            var observer1 = new Mock<IObserve<IEvent>>();
            var observer2 = new Mock<IObserver<IEvent>>();

            eventAggregator.Subscribe(observer1.Object);
            eventAggregator.Subscribe(observer2.Object);

            var testEvent = new TestEvent();
            var otherEvent = new OtherEvent();

            // Act

            eventAggregator.Publish(testEvent);
            eventAggregator.Publish(otherEvent);

            // Assert

            observer1.Verify(x => x.OnNext(It.IsAny<IEvent>()), Times.Exactly(2));
            observer2.Verify(x => x.OnNext(It.IsAny<IEvent>()), Times.Exactly(2));
        }

        public class TestEvent : IEvent { }
        public class OtherEvent : IEvent { }
    }
}
