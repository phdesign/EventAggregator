using System;

namespace EventAggregator
{
    public interface IObserve<T> : IObserver<IEvent> where T : IEvent
    {
        
    }
}