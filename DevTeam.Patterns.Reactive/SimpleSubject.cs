namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Collections.Generic;
    using Dispose;

    internal class SimpleSubject<T>: ISubject<T>
    {
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            _observers.Add(observer);
            return Disposable.Create(() => { _observers.Remove(observer); });
        }

        public void OnNext(T value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }            
        }

        public void OnError(Exception error)
        {
            foreach (var observer in _observers)
            {
                observer.OnError(error);
            }            
        }

        public void OnCompleted()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }

            _observers.Clear();
        }        
    }
}