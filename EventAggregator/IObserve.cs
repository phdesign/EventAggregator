using System;

namespace EventAggregator
{
    public interface IObserve<T> : IEvent
    {
        void Subscribe(IEventAggregator aggregator);
        void Handle(T message);
    }
}