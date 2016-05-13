﻿namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal class Scheduler: IScheduler, IDisposable
    {
        private readonly object _lockObject = new object();
        private readonly Task[] _tasks;
        private readonly Queue<Action> _actions = new Queue<Action>();
        private bool _disposed;

        public Scheduler(TaskFactory taskFactory, int parallelism)
        {
            if (parallelism < 1) throw new ArgumentOutOfRangeException(nameof(parallelism));

            _tasks = new Task[parallelism];
            for (var i = 0; i < _tasks.Length; i++)
            {
                _tasks[i] = taskFactory.StartNew(ThreadEntry);
                _tasks[i].Start();
            }
        }

        public void Schedule<TState>(Action action)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            lock (_lockObject)
            {
                _actions.Enqueue(action);
                Monitor.Pulse(_lockObject);
            }            
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
                    _actions.Enqueue(null);
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

                    action = _actions.Dequeue();
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
