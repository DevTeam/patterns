namespace DevTeam.Patterns.Dispose
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a group of Disposables that are disposed together.
    /// </summary>
    public class CompositeDisposable: IDisposable
    {
		private readonly HashSet<IDisposable> _disposables = new HashSet<IDisposable>();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the CompositeDisposable class.
        /// </summary>
        public CompositeDisposable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the CompositeDisposable class from a group of disposables.
        /// </summary>
        /// <param name="items">The disposables that will be disposed together.</param>
        public CompositeDisposable(IEnumerable<IDisposable> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            foreach (var disposable in items)
	        {
		        _disposables.Add(disposable);
	        }
        }

        /// <summary>
        /// Initializes a new instance of the CompositeDisposable class from a group of disposables.
        /// </summary>
        /// <param name="items">The disposables that will be disposed together.</param>
        public CompositeDisposable(params IDisposable[] items)
            :this((IEnumerable<IDisposable>)items)
        {            
        }

        internal int Count => _disposables.Count;

        /// <summary>
        /// Disposes all disposables in the group and removes them from the group.
        /// </summary>
        public void Dispose()
        {            
            Clear();
            _disposed = true;
        }

        /// <summary>
        /// Adds a disposable to the CompositeDisposable or disposes the disposable if the CompositeDisposable is disposed.
        /// </summary>
        /// <param name="disposable">The disposable to add.</param>
	    public void Add(IDisposable disposable)
	    {
		    if (_disposed)
		    {
				disposable.Dispose();
				return;			    
		    }

		    _disposables.Add(disposable);
	    }

        /// <summary>
        /// Removes and disposes the first occurrence of a disposable from the CompositeDisposable.
        /// </summary>
        /// <param name="disposable">The disposable to remove.</param>
        /// <returns><c>False</c> if has no item.</returns>
	    public bool Remove(IDisposable disposable)
	    {
		    if (!_disposables.Remove(disposable))
		    {
			    return false;
		    }

		    disposable.Dispose();
		    return true;
	    }

        /// <summary>
        /// Removes and disposes all disposables from the GroupDisposable, but does not dispose the CompositeDisposable.
        /// </summary>
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
