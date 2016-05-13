namespace DevTeam.Patterns.Reactive
{
    using System;

    public static class Observable
    {
        public static IObservable<TSource> Create<TSource>(Func<IObserver<TSource>, IDisposable> observable)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));

            return new ObservableCreate<TSource>(observable);
        }

        public static IObservable<TSource> ObserveOn<TSource>(this IObservable<TSource> observable, IScheduler scheduler)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            return Create<TSource>(observer => observable.Subscribe(new ObserverOn<TSource>(observer, scheduler)));
        }

        private class ObservableCreate<TSource> : IObservable<TSource>
        {
            private readonly Func<IObserver<TSource>, IDisposable> _observable;

            public ObservableCreate(Func<IObserver<TSource>, IDisposable> observable)
            {
                if (observable == null) throw new ArgumentNullException(nameof(observable));

                _observable = observable;
            }

            public IDisposable Subscribe(IObserver<TSource> observer)
            {
                if (observer == null) throw new ArgumentNullException(nameof(observer));

                return _observable(observer);
            }
        }

        private class ObserverOn<T> : IObserver<T>
        {
            private readonly IObserver<T> _observable;
            private readonly IScheduler _scheduler;

            public ObserverOn(IObserver<T> observable, IScheduler scheduler)
            {
                if (observable == null) throw new ArgumentNullException(nameof(observable));
                if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

                _observable = observable;
                _scheduler = scheduler;
            }

            public void OnNext(T value)
            {
                _scheduler.Schedule<IObserver<T>>(() => { _observable.OnNext(value); });
            }

            public void OnError(Exception error)
            {
                _scheduler.Schedule<IObserver<T>>(() => { _observable.OnError(error); });
            }

            public void OnCompleted()
            {
                _scheduler.Schedule<IObserver<T>>(() => { _observable.OnCompleted(); });
            }
        }
    }    
}
