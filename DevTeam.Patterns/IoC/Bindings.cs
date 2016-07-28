namespace DevTeam.Patterns.IoC
{
    using System;

    public static class Bindings
    {
        public static IRegistration Bind<T, TImplementation>(this IContainer container, object key = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return container.Bind(typeof(EmptyState), typeof(T), typeof(TImplementation), key);
        }

        public static IRegistration Bind<TState, T, TImplementation>(this IContainer container, object key = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return container.Bind(typeof(TState), typeof(T), typeof(TImplementation), key);
        }

        public static IRegistration Bind(this IContainer container, Type stateType, Type contractType, Type implementationType, object key = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            // Resolve default binder
            var binder = container.Resolve<IBinder>();
            // Resolve default factory
            var factory = container.Resolve<IFactory>();
            
            return binder.Bind(container, stateType, contractType, implementationType, factory, key);
        }
    }
}
