namespace DevTeam.Patterns.Dispose
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A set of extsnsions for disposables.
    /// </summary>
    /// <example>    
    /// <code>
    /// var disposable = Disposable.Create(() => System.Console.WriteLine("Disposed."));
    /// disposable.Dispose();
    /// </code>
    /// </example>
    public static class Disposable
    {
        /// <summary>
        /// Creates disposable which does the action when disposing.
        /// </summary>
        /// <param name="disposeAction"></param>
        /// <returns></returns>
        public static IDisposable Create(Action disposeAction)
        {
            if (disposeAction == null) throw new ArgumentNullException(nameof(disposeAction));

            return new DisposableCreate(disposeAction);
        }

        /// <summary>
        /// Creates disposable which does nothing.
        /// </summary>
        /// <returns></returns>
        public static IDisposable Empty()
        {
            return DisposableEmpty.Shared;
        }

        /// <summary>
        /// Initializes a new instance of the CompositeDisposable class from a group of disposables.
        /// </summary>
        /// <param name="disposables">The disposables that will be disposed together.</param>
        /// <returns>Composite disposable.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDisposable ToCompositeDisposable(this IEnumerable<IDisposable> disposables)
        {
            if (disposables == null) throw new ArgumentNullException(nameof(disposables));

            return new CompositeDisposable(disposables);
        }

        private class DisposableCreate : IDisposable
        {
            private readonly Action _disposeAction;

            public DisposableCreate(Action disposeAction)
            {
                if (disposeAction == null) throw new ArgumentNullException(nameof(disposeAction));

                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                _disposeAction();
            }
        }

        private class DisposableEmpty : IDisposable
        {
            public static readonly IDisposable Shared = new DisposableEmpty();

            private DisposableEmpty()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
