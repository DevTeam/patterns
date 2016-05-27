namespace DevTeam.Patterns.Dispose
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class CompositeDisposable: Collection<IDisposable>, IDisposable
    {
        private bool _disposed = false;

        public CompositeDisposable()
        {
        }

        public CompositeDisposable(IList<IDisposable> list)
            : base(list)
        {
        }

        public void Dispose()
        {
           Clear();
        }

        protected override void ClearItems()
        {
            foreach (var disposable in Items)
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
