using System;

namespace EventAggregator
{
    public interface IEventAggregator
    {
        IDisposable Subscribe<T>(Action<T> handler) where T : IEvent;
        void Publish<T>(T message) where T : IEvent;
    }
}
