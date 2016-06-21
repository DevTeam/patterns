namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Collections.Generic;

    using Dispose;

    public class ManualScheduler: IScheduler, IDisposable
    {
        private readonly LinkedList<Action> _actions = new LinkedList<Action>();

        public IDisposable Schedule(Action action)
        {
            _actions.AddLast(action);
            return Disposable.Create(() => { _actions.Remove(action); });
        }

        public void Dispose()
        {
            Process();
        }

        public int Process(int count = int.MaxValue)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));

            var processed = 0;
            while (_actions.Count > 0 && count > 0)
            {
                try
                {
                    var action = _actions.First.Value;
                    count--;
                    processed++;
                    _actions.RemoveFirst();
                    action();                    
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return processed;
        }
    }
}