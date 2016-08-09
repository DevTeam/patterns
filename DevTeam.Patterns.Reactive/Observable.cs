namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using IoC;

    using Dispose;

    /// <summary>
    /// Observable extsnsions.
    /// </summary>
    public static class Observable
    {
        /// <summary>
        /// Creates new Observable.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="observable"></param>
        /// <returns></returns>
        public static IObservable<TSource> Create<TSource>(
            Func<IObserver<TSource>, IDisposable> observable)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));

            return new ObservableCreate<TSource>(observable);
        }

        public static IDisposable Subscribe<TSource>(
            this IObservable<TSource> observable,
            Action<TSource> onNext,
            Action<Exception> onError,
            Action onComplete)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (onNext == null) throw new ArgumentNullException(nameof(onNext));
            if (onError == null) throw new ArgumentNullException(nameof(onError));
            if (onComplete == null) throw new ArgumentNullException(nameof(onComplete));

            return new Subscription<TSource>(observable, onNext, onError, onComplete);
        }

        public static IObservable<TDestination> Select<TSource, TDestination>(
            this IObservable<TSource> observable,
            Func<TSource, TDestination> selector)
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

        public static IObservable<TSource> Where<TSource>(
            this IObservable<TSource> observable,
            Func<TSource, bool> filter)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            return Create<TSource>(
                observer =>
                {
                    return observable.Subscribe(
                        i =>
                        {
                            if (filter(i))
                            {
                                observer.OnNext(i);
                            }
                        },
                        observer.OnError,
                        observer.OnCompleted);
                });
        }

        public static IObservable<TSource> ObserveOn<TSource>(
            this IObservable<TSource> observable,
            IScheduler scheduler)
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

        public static IObservable<TSource> SubscribeOn<TSource>(
            this IObservable<TSource> observable,
            IScheduler scheduler)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            return Create<TSource>(observer =>
                {
                    var disposable = new CompositeDisposable();
                    scheduler.Schedule(
                        () =>
                            {
                                try
                                {
                                    disposable.Add(observable.Subscribe(observer));
                                }
                                catch (Exception error)
                                {
                                    observer.OnError(error);
                                }
                            });

                    return disposable;
                });
        }


        public static IObservable<TSource> AsObservable<TSource>(
            this TSource value)
        {
            return Create<TSource>(observer =>
            {
                observer.OnNext(value);
                observer.OnCompleted();
                return Disposable.Empty();
            });
        }

        public static IObservable<TSource> AsObservable<TSource>(
            this Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            return Create<TSource>(observer =>
            {
                observer.OnError(error);
                return Disposable.Empty();
            });
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

        public static IObservable<TSource> ToObservable<TSource>(
            this IEnumerable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

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

        public static IObservable<TSource> ToObservable<TSource>(
            params TSource[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.ToObservable();
        }

        public static IObservable<TResult> ToObservable<TResult>(
            this Task<TResult> task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            return ToObservable(task, t => t.Result);
        }

        public static IObservable<EmptyState> ToObservable(
            this Task task)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            return ToObservable(task, t => EmptyState.Shared);
        }

        public static IObservable<TResult> SelectMany<TSource, TResult>(
            this IObservable<TSource> source,
            Func<TSource, IObservable<TResult>> selector)
            where TSource : IObservable<TResult>
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return SelectMany(
                source,
                src => selector(src).Materialize().Where(i => i.EventType == Event<TResult>.Type.OnNext).Dematerialize(),
                e => e.AsObservable<TResult>(),
                Empty<TResult>);
        }

        private static IObservable<TResult> SelectMany<TSource, TResult>(
            this IObservable<TSource> source,
            Func<TSource, IObservable<TResult>> onNext,
            Func<Exception, IObservable<TResult>> onError,
            Func<IObservable<TResult>> onCompleted)
            where TSource : IObservable<TResult>
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (onNext == null) throw new ArgumentNullException(nameof(onNext));
            if (onError == null) throw new ArgumentNullException(nameof(onError));
            if (onCompleted == null) throw new ArgumentNullException(nameof(onCompleted));

            return Create<TResult>(
                observer =>
                    {
                        var disposableOnNext = new SerialDisposable();
                        var disposableOnError = new SerialDisposable();
                        var disposableOnCompleted = new SerialDisposable();

                        source.Subscribe(
                            i =>
                            {
                                disposableOnNext.Disposable = onNext(i).Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted);
                            },
                            e =>
                            {
                                disposableOnError.Disposable = onError(e).Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted);
                            },
                            () =>
                            {
                                disposableOnCompleted.Disposable = onCompleted().Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted);
                            });

                        return new CompositeDisposable(disposableOnNext, disposableOnError, disposableOnCompleted);
                    });
        }

        private static IObservable<TResult> ToObservable<TTask, TResult>(
            this TTask task,
            Func<TTask, TResult> resultSelector)
            where TTask : Task
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return Create<TResult>(observer =>
            {
                try
                {
                    task.Wait();

                    if (task.IsFaulted)
                    {
                        observer.OnError(task.Exception);
                        return Disposable.Empty();
                    }

                    if (task.IsCompleted)
                    {
                        observer.OnNext(resultSelector(task));
                    }

                    observer.OnCompleted();
                }
                catch (AggregateException ex)
                {
                    observer.OnError(ex.InnerException);
                }

                return Disposable.Empty();
            });
        }

        public static IObservable<TResult> ToObservable<TResult>(
            this IEnumerable<Task<TResult>> tasks)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));

            var observable = Empty<TResult>();
            return tasks.Aggregate(observable, (current, task) => current.Concat(task.ToObservable()));
        }

        public static IObservable<EmptyState> ToObservable(
            this IEnumerable<Task> tasks)
        {
            if (tasks == null) throw new ArgumentNullException(nameof(tasks));

            var observable = Empty<EmptyState>();
            return tasks.Aggregate(observable, (current, task) => current.Concat(task.ToObservable()));
        }

        public static void WaitForCompletion<TSource>(
            this IObservable<TSource> observable)
        {
            if (observable == null) throw new ArgumentNullException(nameof(observable));

            var lockObject = new object();
            var isCompleted = false;
            Exception error = null;
            var subscription = observable.Subscribe(
                i => { },
                e => {
                    lock (lockObject)
                    {
                        isCompleted = true;
                        error = e;
                        Monitor.Pulse(lockObject);
                    }
                },
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

            if (error != null)
            {
                throw error;
            }
        }

        public static IObservable<TSource> Concat<TSource>(
            this IObservable<TSource> source1,
            IObservable<TSource> source2)
        {
            if (source1 == null) throw new ArgumentNullException(nameof(source1));
            if (source2 == null) throw new ArgumentNullException(nameof(source2));

            return Create<TSource>(
                observer =>
                    {
                        var disposable = new CompositeDisposable();
                        disposable.Add(
                            source1.Subscribe(
                                observer.OnNext,
                                observer.OnError,
                                () =>
                                {
                                    disposable.Add(source2.Subscribe(observer.OnNext, observer.OnError, observer.OnCompleted));
                                }));

                        return disposable;
                    });
        }

        public static IObservable<Event<TSource>> Materialize<TSource>(
            this IObservable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return Create<Event<TSource>>(
                observer =>
                    {
                        return source.Subscribe(
                            i => observer.OnNext(Event<TSource>.CreateOnNext(i)),
                            e =>
                                {
                                    observer.OnNext(Event<TSource>.CreateOnError(e));
                                    observer.OnCompleted();
                                },
                            () =>
                                {
                                    observer.OnNext(Event<TSource>.CreateOnComplete());
                                    observer.OnCompleted();
                                });
                    });
        }

        public static IObservable<TSource> Dematerialize<TSource>(
            this IObservable<Event<TSource>> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return Create<TSource>(observer =>
            {
                return source.Subscribe(i =>
                {
                    switch (i.EventType)
                    {
                        case Event<TSource>.Type.OnNext:
                            observer.OnNext(i.Value);
                            break;

                        case Event<TSource>.Type.OnError:
                            observer.OnError(i.Error);
                            break;

                        case Event<TSource>.Type.OnComplete:
                            observer.OnCompleted();
                            break;
                    }
                },
                    e => { },
                    () => { });
            });
        }

        public static IEnumerable<TSource> ToEnumerable<TSource>(
            this IObservable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return new ObservableEnumerable<TSource>(source);
        }

        private class ObservableEnumerable<TSource> : IEnumerable<TSource>
        {
            private readonly IObservable<TSource> _source;

            public ObservableEnumerable(
                IObservable<TSource> source)
            {
                if (source == null) throw new ArgumentNullException(nameof(source));

                _source = source;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public IEnumerator<TSource> GetEnumerator()
            {
                return new ObservableEnumerator<TSource>(_source);
            }
        }

        private class ObservableEnumerator<TSource> : IEnumerator<TSource>
        {
            private readonly IObservable<TSource> _source;
            private readonly LinkedList<Event<TSource>> _events = new LinkedList<Event<TSource>>();
            private IDisposable _disposable = Disposable.Empty();
            private Event<TSource> _current;

            public ObservableEnumerator(
                IObservable<TSource> source)
            {
                if (source == null) throw new ArgumentNullException(nameof(source));

                _source = source;
                Subscribe();
            }

            object IEnumerator.Current => _current.Value;

            public TSource Current => _current.Value;

            public bool MoveNext()
            {
                lock (_events)
                {
                    while (_events.Count == 0)
                    {
                        Monitor.Wait(_events);
                    }

                    _current = _events.First.Value;
                    _events.RemoveFirst();
                    switch (_current.EventType)
                    {
                        case Event<TSource>.Type.OnNext:
                            return true;

                        case Event<TSource>.Type.OnComplete:
                            return false;

                        case Event<TSource>.Type.OnError:
                            throw _current.Error;
                    }
                }

                return true;
            }

            public void Reset()
            {
                Subscribe();
            }

            public void Dispose()
            {
                _disposable.Dispose();
            }

            private void Subscribe()
            {
                _disposable.Dispose();
                _events.Clear();
                _disposable = _source.Materialize().Subscribe(AddEvent, e => { }, () => { });
            }

            private void AddEvent(Event<TSource> ev)
            {
                if (ev == null) throw new ArgumentNullException(nameof(ev));

                lock (_events)
                {
                    _events.AddLast(ev);
                    Monitor.Pulse(_events);
                }
            }
        }

        private class ObservableCreate<TSource> : IObservable<TSource>
        {
            private readonly Func<IObserver<TSource>, IDisposable> _observable;

            public ObservableCreate(
                Func<IObserver<TSource>, IDisposable> observable)
            {
                if (observable == null) throw new ArgumentNullException(nameof(observable));

                _observable = observable;
            }

            public IDisposable Subscribe(
                IObserver<TSource> observer)
            {
                if (observer == null) throw new ArgumentNullException(nameof(observer));

                return _observable(observer);
            }
        }

        private class ObserverOn<T> : IObserver<T>
        {
            private readonly IObserver<T> _observer;
            private readonly IScheduler _scheduler;

            public ObserverOn(
                IObserver<T> observer,
                IScheduler scheduler)
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

            public Subscription(
                IObservable<T> observable,
                Action<T> onNext,
                Action<Exception> onError,
                Action onComplete)
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
