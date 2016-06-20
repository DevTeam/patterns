namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Dispose;

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

        public static IObserver<TSource> ObserveOn<TSource>(this IObserver<TSource> observer, IScheduler scheduler)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            return new ObserverOn<TSource>(observer, scheduler);            
        }

        public static IObservable<TSource> SubscribeOn<TSource>(this IObservable<TSource> observable, IScheduler scheduler)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            return Create<TSource>(observer =>
                {
                    var disposable = new CompositeDisposable();
                    scheduler.Schedule(() => disposable.Add(observable.Subscribe(observer)));
                    return disposable;
                });
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

        public static void WaitForCompletion<TSource>(this IObservable<TSource> observable)
        {
            var lockObject = new object();
            var isCompleted = false;
            var subscription = observable.Subscribe(
                i => { }, 
                e => { },
                () =>
                    {
                        lock (lockObject)
                        {
                            isCompleted = true;
                            Monitor.Pulse(lockObject);
                        }
                    });

            lock (lockObject)
            {
                while (!isCompleted)
                {
                    Monitor.Wait(lockObject);
                }

                subscription.Dispose();
            }           
        }


        public static IObservable<TSource> Empty<TSource>()
        {
            return Create<TSource>(
                observer =>
                {
                    observer.OnCompleted();
                    return Disposable.Empty();
                });
        }

        public static IObservable<TSource> Concat<TSource>(this IObservable<TSource> source1, IObservable<TSource> source2)
        {
            return Create<TSource>(
                observer =>
                    {
                        var disposable = new CompositeDisposable();
                        disposable.Add(
                            source1.Subscribe(observer.OnNext, observer.OnError, () =>
                                {
                                    disposable.Add(source2.Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted));
                                }));

                        return disposable;
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
            private readonly IObserver<T> _observer;
            private readonly IScheduler _scheduler;

            public ObserverOn(IObserver<T> observer, IScheduler scheduler)
            {
                if (observer == null) throw new ArgumentNullException(nameof(observer));
                if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

                _observer = observer;
                _scheduler = scheduler;
            }

            public void OnNext(T value)
            {
                _scheduler.Schedule(() => { _observer.OnNext(value); });
            }

            public void OnError(Exception error)
            {
                _scheduler.Schedule(() => { _observer.OnError(error); });
            }

            public void OnCompleted()
            {
                _scheduler.Schedule(() => { _observer.OnCompleted(); });
            }
        }

        private class Subscription<T> : IObserver<T>, IDisposable
        {
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
