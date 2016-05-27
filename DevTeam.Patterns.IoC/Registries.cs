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

        public static IRegistry Using<TContext>(this IRegistry registry, Func<TContext> context)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (context == null) throw new ArgumentNullException(nameof(context));

            return new ContextRegistry<TContext>(registry, context);
        }

        public static IRegistry Using<TContext>(this IContainer container, string name)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return new ContextRegistry<TContext>(container, () => container.Resolve<TContext>(name));
        }

        private class ContextRegistry<TContext> : IRegistry
        {
            private readonly IRegistry _registry;
            private readonly Func<TContext> _context;            

            public ContextRegistry(IRegistry registry, Func<TContext> context)
            {
                if (registry == null) throw new ArgumentNullException(nameof(registry));
                if (context == null) throw new ArgumentNullException(nameof(context));                

                _registry = registry;
                _context = context;                
            }

            public IDisposable Register(Type stateType, Type instanceType, Func<object, object> factoryMethod, string name = "")
            {
                using (new Context())
                using (Context.Instance.Register(_context))
                {
                    return _registry.Register(stateType, instanceType, factoryMethod, name);
                }                
            }
        }
    }    
}