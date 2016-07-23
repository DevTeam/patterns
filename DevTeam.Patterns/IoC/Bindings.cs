namespace DevTeam.Patterns.IoC
{
    using System;

    public static class Bindings
    {
        public static IRegistration Bind<T, TImplementation>(this IContainer container, object key = null)
        {
            return container.Bind(typeof(EmptyState), typeof(T), typeof(TImplementation), key);
        }

        public static IRegistration Bind<TState, T, TImplementation>(this IContainer container, object key = null)
        {
            return container.Bind(typeof(TState), typeof(T), typeof(TImplementation), key);
        }

        public static IRegistration Bind(this IContainer container, Type stateType, Type instanceType, Type implementationType, object key = null)
        {
            var binder = container.Resolve<IBinder>();
            return binder.Bind(container, stateType, instanceType, implementationType, key);
        }
    }
}
