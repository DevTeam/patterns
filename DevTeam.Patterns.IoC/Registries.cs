namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

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
            where TContext: IContext
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (contextName == null) throw new ArgumentNullException(nameof(contextName));

            return container.Using(() => container.Resolve<TContext>(contextName));
        }

        private static IContainer Using<TContext>(this IContainer container, Func<TContext> factoryMethod)
            where TContext : IContext
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            return new RegisterContainer<TContext>(container, factoryMethod);           
        }

        private class RegisterContainer<TContext> : IContainer
            where TContext: IContext
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
	            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
	            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
	            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
	            if (name == null) throw new ArgumentNullException(nameof(name));

	            using (_container.Register(_factoryMethod))
                {
                    return _container.Register(stateType, instanceType, factoryMethod, name);
                }
            }

            public object Resolve(Type stateType, Type instanceType, object state, string name = "")
            {
	            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
	            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
	            if (state == null) throw new ArgumentNullException(nameof(state));
	            if (name == null) throw new ArgumentNullException(nameof(name));

	            return _container.Resolve(stateType, instanceType, state, name);
            }

	        public IEnumerable<Tuple<IRegistryKey, object>> Resolve(Func<IRegistryKey, bool> filter, Func<IRegistryKey, object> stateSelector)
	        {
		        if (filter == null) throw new ArgumentNullException(nameof(filter));
		        if (stateSelector == null) throw new ArgumentNullException(nameof(stateSelector));

		        return _container.Resolve(filter, stateSelector);
	        }

	        public void Dispose()
            {
                _container.Dispose();
            }
        }
    }    
}