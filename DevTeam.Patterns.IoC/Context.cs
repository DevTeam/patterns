namespace DevTeam.Patterns.IoC
{
    using System;

    internal class Context: IDisposable
    {
        [ThreadStatic] private static IContainer _container;
        private readonly IContainer _prevContainer;

        public static IContainer Instance
        {
            get
            {
                if (_container == null)
                {
                    _container = new DefaultContextContainerConfiguration().Apply(new Container(nameof(Context), false));
                }

                return _container;
            }
        }

        public Context()
        {
            _prevContainer = Instance;
            _container = _container.Resolve<IContainer>();
        }

        public void Dispose()
        {
            _container = _prevContainer;
        }
    }
}