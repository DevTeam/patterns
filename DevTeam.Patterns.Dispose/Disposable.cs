namespace DevTeam.Patterns.Dispose
{
    using System;

    public static class Disposable
    {
        public static IDisposable Create<TState>(TState state, Action<TState> disposeAction)
        {
            if (disposeAction == null) throw new ArgumentNullException(nameof(disposeAction));
            return new DisposableCreate<TState>(state, disposeAction);
        }

        public static IDisposable Empty()
        {
            return DisposableEmpty.Shared;
        }

        private class DisposableCreate<TState>: IDisposable
        {
            private readonly TState _state;
            private readonly Action<TState> _disposeAction;

            public DisposableCreate(TState state, Action<TState> disposeAction)
            {
                if (disposeAction == null) throw new ArgumentNullException(nameof(disposeAction));
                _state = state;
                _disposeAction = disposeAction;                
            }

            public void Dispose()
            {
                _disposeAction(_state);
            }
        }

        private class DisposableEmpty : IDisposable
        {
            public static readonly IDisposable Shared = new DisposableEmpty();

            public void Dispose()
            {
            }
        }
    }
}
