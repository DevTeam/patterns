namespace DevTeam.Patterns.Dispose
{
    using System;

    public class SerialDisposable: IDisposable
    {
        private IDisposable _disposable = Patterns.Dispose.Disposable.Empty();

        public SerialDisposable()            
        {
        }

        public SerialDisposable(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));

            Disposable = disposable;
        }

        public void Dispose()
        {
            IsDisposed = true;
            Disposable.Dispose();
        }

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

        public bool IsDisposed { get; private set; }
    }
}
