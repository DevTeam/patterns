namespace DevTeam.Patterns.EventAggregator
{
    using System;
    using System.Collections.Generic;

    using Reactive;

    internal class Aggregator : IEventAggregator
    {
        private readonly Dictionary<Type, object> _subjects = new Dictionary<Type, object>();

        public IDisposable RegisterProvider<T>(IObservable<T> provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            System.Diagnostics.Debug.WriteLine($"{nameof(Aggregator)}: Register provider for {typeof(T).Name}");

            var subject = GetSubject<T>();
            return provider.Subscribe(subject);
        }

        public IDisposable RegisterConsumer<T>(IObserver<T> consumer)
        {
            if (consumer == null) throw new ArgumentNullException(nameof(consumer));

            System.Diagnostics.Debug.WriteLine($"{nameof(Aggregator)}: Register consumer for {typeof(T).Name}");

            var subject = GetSubject<T>();
            return subject.Subscribe(consumer);
        }       

        private Subject<T> GetSubject<T>()
        {
            object subject;
            if (!_subjects.TryGetValue(typeof(T), out subject))
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(Aggregator)}: Create subject for {typeof(T).Name}");
                subject = new Subject<T>();
                _subjects.Add(typeof(T), subject);
            }

            return (Subject<T>)subject;
        }
    }
}
