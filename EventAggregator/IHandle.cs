using System;

namespace EventAggregator
{
    public interface IHandle<T> : IEvent
    {
        void Handle(T message);
    }
}