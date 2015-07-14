using System;

namespace EventAggregator
{
    public interface IEventAggregator : IObservable<IEvent>
    {
        void Publish(IEvent message);
    }
}
