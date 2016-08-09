namespace DevTeam.Patterns.Dispose
{
    using System;

    /// <summary>
    /// Represents a disposable whose underlying disposable can be swapped for another disposable which causes the previous underlying disposable to be disposed.
    /// </summary>
    public class SerialDisposable : IDisposable
    {
        private IDisposable _disposable = Patterns.Dispose.Disposable.Empty();

        /// <summary>
        /// Initializes a new instance of the SerialDisposable class.
        /// </summary>
        public SerialDisposable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SerialDisposable class and initialize the underlying disposable.
        /// </summary>
        /// <param name="disposable">The underlying disposable.</param>
        public SerialDisposable(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));

            Disposable = disposable;
        }

        /// <summary>
        /// Disposes all disposables in the group and removes them from the group.
        /// </summary>
        public void Dispose()
        {
            IsDisposed = true;
            Disposable.Dispose();
        }

        /// <summary>
        /// Gets or sets the underlying disposable.
        /// </summary>
        /// <remarks>
        /// If the SerialDisposable has already been disposed, assignment to this property causes immediate disposal of the given disposable object. Assigning this property disposes the previous disposable object.
        /// </remarks>
        public IDisposable Disposable
        {
            get
            {
                return _disposable;
            }

            set
            {
                if (IsDisposed)
                {
                    _disposable = value;
                    _disposable.Dispose();
                }
                else
                {
                    _disposable.Dispose();
                    _disposable = value;
                }
            }
        }

        private bool IsDisposed { get; set; }
    }
}
