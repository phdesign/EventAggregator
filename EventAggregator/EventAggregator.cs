using System;
using System.Collections.Generic;

namespace EventAggregator
{
    public class EventAggregator : IEventAggregator
    {
        private readonly List<IEvent> _events;
        private readonly List<IObserver<IEvent>> _observers;

        public EventAggregator()
        {
            _events = new List<IEvent>();
            _observers = new List<IObserver<IEvent>>();
        }

        public IDisposable Subscribe(IObserver<IEvent> observer)
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

        public void Publish(IEvent message)
        {
            _events.Add(message);
            foreach (var observer in _observers)
                observer.OnNext(message);
        }
    }

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