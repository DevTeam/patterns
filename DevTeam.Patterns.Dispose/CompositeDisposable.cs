namespace DevTeam.Patterns.Dispose
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class CompositeDisposable: Collection<IDisposable>, IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private bool _disposed = false;

        public CompositeDisposable(params IDisposable[] disposables)
        {
            _disposables.AddRange(disposables);
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
        }

        protected override void ClearItems()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            base.ClearItems();
        }

        protected override void InsertItem(int index, IDisposable item)
        {
            if (_disposed)
            {
                item.Dispose();
            }
            else
            {
                base.InsertItem(index, item);
            }            
        }

        protected override void RemoveItem(int index)
        {
            throw new NotImplementedException();
        }

        protected override void SetItem(int index, IDisposable item)
        {
            throw new NotImplementedException();
        }
    }
}
