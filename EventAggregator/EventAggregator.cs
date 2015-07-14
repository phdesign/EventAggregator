using System;
using System.Collections.Generic;
using System.Linq;

namespace EventAggregator
{
    public class EventAggregator : IEventAggregator
    {
        private readonly List<IEvent> _events;
        private readonly List<IObserver<IEvent>> _observers;
        private readonly Dictionary<Type, List<IObserver<IEvent>>> _typedObservers;

        public EventAggregator()
        {
            _events = new List<IEvent>();
            _observers = new List<IObserver<IEvent>>();
            _typedObservers = new Dictionary<Type, List<IObserver<IEvent>>>();
        }

        /// <summary>
        /// Subscribes to all events (in the case of IObserver&lt;IEvent&gt;)
        /// </summary>
        /// <param name="observer">The subscriber.</param>
        /// <returns>An Unsubscriber to allow unsubscribing from events</returns>
        public IDisposable Subscribe(IObserver<IEvent> observer)
        {
            return SubscribeAll(observer);
        }

        /// <summary>
        /// Subscribes to all events (in the case of IObserve&lt;IEvent&gt;)
        /// </summary>
        /// <param name="observer">The subscriber.</param>
        /// <returns>An Unsubscriber to allow unsubscribing from events</returns>
        public IDisposable Subscribe(IObserve<IEvent> observer)
        {
            return SubscribeAll(observer);
        }
        
        /// <summary>
        /// Subscribes to a specific event type.
        /// </summary>
        /// <typeparam name="T">The event type to subscribe to.</typeparam>
        /// <param name="observer">The subscriber.</param>
        /// <returns>An Unsubscriber to allow unsubscribing from events</returns>
        public IDisposable Subscribe<T>(IObserve<T> observer) where T : IEvent
        {
            // Check whether observer is already registered. If not, add it 
            List<IObserver<IEvent>> observers;
            if (!_typedObservers.TryGetValue(typeof(T), out observers))
                observers = _typedObservers[typeof(T)] = new List<IObserver<IEvent>>();
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
                // Provide observer with existing data. 
                foreach (var item in _events.Where(x => x is T))
                    observer.OnNext(item);
            }
            return new Unsubscriber<IEvent>(_typedObservers[typeof(T)], observer);
        }

        /// <summary>
        /// Publishes an event to subscribers
        /// </summary>
        /// <param name="message">The event to publish, can be any implementation of IEvent.</param>
        public void Publish(IEvent message)
        {
            _events.Add(message);
            foreach (var observer in _observers)
                observer.OnNext(message);

            // Publish event to typed observers.
            if (!_typedObservers.ContainsKey(message.GetType())) return;
            foreach (var observer in _typedObservers[message.GetType()])
                observer.OnNext(message);
        }

        /// <summary>
        /// Subscribes to all events
        /// </summary>
        /// <param name="observer">The subscriber.</param>
        /// <returns>An Unsubscriber to allow unsubscribing from events</returns>
        private IDisposable SubscribeAll(IObserver<IEvent> observer)
        {
            // Check whether observer is already registered. If not, add it 
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
                // Provide observer with existing data. 
                foreach (var item in _events)
                    observer.OnNext(item);
            }
            return new Unsubscriber<IEvent>(_observers, observer);
        }
    }

    /// <summary>
    /// Enables a subscriber to unsubscribe to events by calling Dispose().
    /// </summary>
    internal class Unsubscriber<T> : IDisposable
    {
        private readonly List<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        internal Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}