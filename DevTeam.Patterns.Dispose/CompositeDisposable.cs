namespace DevTeam.Patterns.Dispose
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CompositeDisposable: IDisposable
    {
		private readonly HashSet<IDisposable> _disposables = new HashSet<IDisposable>();
        private bool _disposed;

        public CompositeDisposable()
        {
        }

        public CompositeDisposable(IEnumerable<IDisposable> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var disposable in items)
	        {
		        _disposables.Add(disposable);
	        }
        }

        public CompositeDisposable(params IDisposable[] items)
            :this((IEnumerable<IDisposable>)items)
        {            
        }

        internal int Count => _disposables.Count;

        public void Dispose()
        {            
            Clear();
            _disposed = true;
        }

	    public void Add(IDisposable disposable)
	    {
		    if (_disposed)
		    {
				disposable.Dispose();
				return;			    
		    }

		    _disposables.Add(disposable);
	    }

	    public bool Remove(IDisposable disposable)
	    {
		    if (!_disposables.Remove(disposable))
		    {
			    return false;
		    }

		    disposable.Dispose();
		    return true;
	    }

	    public void Clear()
	    {
		    foreach (var disposable in _disposables.Reverse())
		    {
				disposable.Dispose();
		    }

			_disposables.Clear();		    
	    }
    }
}
