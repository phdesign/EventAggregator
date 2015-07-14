using System;
using System.Collections.Generic;
using System.Linq;

namespace EventAggregator
{
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Action<IEvent>>> _handlers = new Dictionary<Type, List<Action<IEvent>>>();

        public IDisposable Subscribe<T>(Action<T> handler) where T : IEvent
        {
            // Check whether handler is already registered. If not, add it 
            List<Action<IEvent>> handlers;
            if (!_handlers.TryGetValue(typeof(T), out handlers))
                handlers = _handlers[typeof(T)] = new List<Action<IEvent>>();
            Action<IEvent> surrogate = x => handler((T)x);
            handlers.Add(surrogate);
            return new Unsubscriber(handlers, surrogate);
        }

        /// <summary>
        /// Publishes an event to subscribers
        /// </summary>
        /// <param name="message">The event to publish, can be any implementation of IEvent.</param>
        public void Publish<T>(T message) where T : IEvent
        {
            if (!_handlers.ContainsKey(message.GetType())) return;
            foreach (var handler in _handlers[message.GetType()])
                handler(message);
        }
    }

    /// <summary>
    /// Enables a subscriber to unsubscribe to events by calling Dispose().
    /// </summary>
    internal class Unsubscriber : IDisposable
    {
        private readonly List<Action<IEvent>> _handlers;
        private readonly Action<IEvent> _handler;

        internal Unsubscriber(List<Action<IEvent>> handlers, Action<IEvent> handler)
        {
            _handlers = handlers;
            _handler = handler;
        }

        public void Dispose()
        {
            if (_handlers.Contains(_handler))
                _handlers.Remove(_handler);
        }
    }
}