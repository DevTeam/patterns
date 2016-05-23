namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Collections.Generic;

    using DevTeam.Patterns.Dispose;

    public static class Observable
    {
        public static IObservable<TSource> Create<TSource>(Func<IObserver<TSource>, IDisposable> observable)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));

            return new ObservableCreate<TSource>(observable);
        }

        public static IDisposable Subscribe<TSource>(this IObservable<TSource> observable, Action<TSource> onNext, Action<Exception> onError, Action onComplete)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (onNext == null) throw new ArgumentNullException(nameof(onNext));
            if (onError == null) throw new ArgumentNullException(nameof(onError));
            if (onComplete == null) throw new ArgumentNullException(nameof(onComplete));

            return new Subscription<TSource>(observable, onNext, onError, onComplete);                
        }

        public static IObservable<TDestination> Select<TSource, TDestination>(this IObservable<TSource> observable, Func<TSource, TDestination> selector)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return Create<TDestination>(
                observer =>
                    {
                        return observable.Subscribe(
                            i => observer.OnNext(selector(i)),
                            observer.OnError,
                            observer.OnCompleted);
                    });
        }

        public static IObservable<TSource> ObserveOn<TSource>(this IObservable<TSource> observable, IScheduler scheduler)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            return Create<TSource>(observer => observable.Subscribe(new ObserverOn<TSource>(observer, scheduler)));
        }

        public static IObservable<TSource> ToObservable<TSource>(this IEnumerable<TSource> source)
        {
            return Create<TSource>(observer =>
                {
                    foreach (var value in source)
                    {
                        observer.OnNext(value);
                    }

                    observer.OnCompleted();
                    return Disposable.Empty();                        
                });
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

        private class Subscription<T> : IObserver<T>, IDisposable
        {
            private readonly IObservable<T> _observable;
            private readonly Action<T> _onNext;
            private readonly Action<Exception> _onError;
            private readonly Action _onComplete;
            private readonly IDisposable _subscription;

            public Subscription(IObservable<T> observable, Action<T> onNext, Action<Exception> onError, Action onComplete)
            {
                if (observable == null) throw new ArgumentNullException(nameof(observable));
                if (onNext == null) throw new ArgumentNullException(nameof(onNext));
                if (onError == null) throw new ArgumentNullException(nameof(onError));
                if (onComplete == null) throw new ArgumentNullException(nameof(onComplete));

                _observable = observable;
                _onNext = onNext;
                _onError = onError;
                _onComplete = onComplete;
                _subscription = observable.Subscribe(this);
            }

            public void OnNext(T value)
            {
                _onNext(value);
            }

            public void OnError(Exception error)
            {
                _onError(error);
            }

            public void OnCompleted()
            {
                _onComplete();
            }

            public void Dispose()
            {
                _subscription.Dispose();
            }
        }
    }    
}
