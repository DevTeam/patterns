namespace IoC.Contracts
{
    using System;

    public static class Factories
    {
        public static IFactory Create(Func<IResolving, object> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            return new FactoryImpl(factory);
        }

        private class FactoryImpl : IFactory
        {
            private readonly Func<IResolving, object> _factory;

            public FactoryImpl(Func<IResolving, object> factory)
            {
                if (factory == null) throw new ArgumentNullException(nameof(factory));
                _factory = factory;
            }

            public object Create(IResolving resolving)
            {
                return _factory(resolving);
            }
        }
    }
}
