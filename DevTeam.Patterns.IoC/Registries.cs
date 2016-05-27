namespace DevTeam.Patterns.IoC
{
    using System;

    public static class Registries
    {
        public static IDisposable Register<TState, T>(this IRegistry registry, Func<TState, T> factoryMethod, string name = "")
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return registry.Register(typeof(TState), typeof(T), state => factoryMethod((TState)state), name);
        }

        public static IDisposable Register<T>(this IRegistry registry, Func<T> factoryMethod, string name = "")
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return registry.Register(new Func<EmptyState, T>(ignoredArg => factoryMethod()), name);
        }

        public static IContainer Using<TContext>(this IContainer container, string contextName)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (contextName == null) throw new ArgumentNullException(nameof(contextName));

            return container.Using(() => container.Resolve<TContext>(contextName));
        }

        public static IContainer Using<TContext>(this IContainer container, Func<TContext> factoryMethod)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

                return new RegisterContainer<TContext>(container, factoryMethod);           
        }

        private class RegisterContainer<TContext> : IContainer
        {
            private readonly IContainer _container;
            private readonly Func<TContext> _factoryMethod;

            public RegisterContainer(IContainer container, Func<TContext> factoryMethod)
            {
                if (container == null) throw new ArgumentNullException(nameof(container));
                if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

                _container = container;
                _factoryMethod = factoryMethod;
            }

            public string Name => _container.Name;

            public IDisposable Register(Type stateType, Type instanceType, Func<object, object> factoryMethod, string name = "")
            {
                using (_container.Register(_factoryMethod))
                {
                    return _container.Register(stateType, instanceType, factoryMethod, name);
                }
            }

            public object Resolve(Type stateType, Type instanceType, object state, string name = "")
            {
                return _container.Resolve(stateType, instanceType, state, name);
            }

            public void Dispose()
            {
                _container.Dispose();
            }
        }
    }    
}