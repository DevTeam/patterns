namespace DevTeam.Patterns.EventAggregator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Dispose;

    using IoC;

    using Reactive;

    internal class Aggregator : IEventAggregator
    {
        private readonly IResolver _resolver;
        private readonly Dictionary<Type, object> _subjects = new Dictionary<Type, object>();

        public Aggregator(IResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            _resolver = resolver;
        }

        public IDisposable RegisterProvider<T>(IObservable<T> provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            WriteLog($"Register provider for {typeof(T).Name}");

            var subject = GetSubject<T>();
            return new CompositeDisposable(
                provider.Subscribe(subject),
                Disposable.Create(() => { WriteLog($"Unregister provider for {typeof(T).Name}"); }));
        }

        public IDisposable RegisterConsumer<T>(IObserver<T> consumer)
        {
            if (consumer == null) throw new ArgumentNullException(nameof(consumer));

            WriteLog($"Register consumer for {typeof(T).Name}");

            var subject = GetSubject<T>();
            return new CompositeDisposable(
                subject.Subscribe(consumer),
                Disposable.Create(() => { WriteLog($"Unregister consumer for {typeof(T).Name}"); }));
        }

        public override string ToString()
        {
            return $"{nameof(Aggregator)} [Subjects: {_subjects.Count}]";
        }

        private ISubject<T> GetSubject<T>()
        {
            object subject;
            if (!_subjects.TryGetValue(typeof(T), out subject))
            {
                WriteLog($"Create subject for {typeof(T).Name}");
                subject = _resolver.Resolve<ISubject<T>>(WellknownSubject.Simple);
                _subjects.Add(typeof(T), subject);
            }

            return (ISubject<T>)subject;
        }

        [Conditional("DEBUG")]
        private void WriteLog(string message)
        {
            Debug.WriteLine($"{nameof(Aggregator)}: {message}");
        }
    }
}
