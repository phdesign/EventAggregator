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

        public void ObserverStopsReceivingEventsWhenUnsubscribed()
        {
            
        }

        public void BaseObserverReceivesAllEvents()
        {
            
        }

        public class TestEvent : IEvent { }
        public class OtherEvent : IEvent { }
    }
}
