namespace DevTeam.Patterns.Dispose
{
    using System;

    public static class Disposable
    {
        public static IDisposable Create(Action disposeAction)
        {
            if (disposeAction == null) throw new ArgumentNullException(nameof(disposeAction));

            return new DisposableCreate(disposeAction);
        }

        public static IDisposable Empty()
        {
            return DisposableEmpty.Shared;
        }

        private class DisposableCreate: IDisposable
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
