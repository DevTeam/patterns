namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Dispose;

    internal class Scheduler : IScheduler, IDisposable
    {
        private readonly object _lockObject = new object();
        private readonly Task[] _tasks;
        private readonly LinkedList<Action> _actions = new LinkedList<Action>();
        private bool _disposed;

        public Scheduler(TaskFactory taskFactory, int parallelism)
        {
            if (taskFactory == null) throw new ArgumentNullException(nameof(taskFactory));
            if (parallelism < 1) throw new ArgumentOutOfRangeException(nameof(parallelism));

            _tasks = new Task[parallelism];
            for (var i = 0; i < _tasks.Length; i++)
            {
                _tasks[i] = taskFactory.StartNew(ThreadEntry);
            }
        }

        public IDisposable Schedule(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (_disposed) throw new ObjectDisposedException(GetType().Name);

            lock (_lockObject)
            {
                _actions.AddFirst(action);
                Monitor.Pulse(_lockObject);
            }

            return Disposable.Create(() =>
                {
                    lock (_lockObject)
                    {
                        _actions.Remove(action);
                    }
                });
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            foreach (var task in _tasks)
            {
                lock (_lockObject)
                {
                    _actions.AddFirst((Action)null);
                    Monitor.Pulse(_lockObject);
                }
            }

            foreach (var task in _tasks)
            {
                try
                {
                    task.Wait();
                }
                catch
                {
                    // ignored
                }
            }
        }

        public override string ToString()
        {
            lock (_lockObject)
            {
                return $"{nameof(Scheduler)} [Parallelism: {_tasks.Length}, Actions: {_actions.Count}, Disposed: {_disposed}]";
            }
        }

        private void ThreadEntry()
        {
            do
            {
                Action action;
                lock (_lockObject)
                {
                    while (_actions.Count == 0)
                    {
                        Monitor.Wait(_lockObject);
                    }

                    var last = _actions.Last;
                    _actions.RemoveLast();
                    action = last.Value;
                }

                try
                {
                    if (action != null)
                    {
                        action();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            while (true);
        }
    }
}
