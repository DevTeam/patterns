namespace DevTeam.Patterns.IoC
{
    using System;

    public static class Bindings
    {
        public static IDisposable Map<T, TImplementation>(this IContainer container, object key = null)
        {
            return container.Map(typeof(EmptyState), typeof(T), typeof(TImplementation), key);
        }

        public static IDisposable Map<TState, T, TImplementation>(this IContainer container, object key = null)
        {
            return container.Map(typeof(TState), typeof(T), typeof(TImplementation), key);
        }

        public static IDisposable Map(this IContainer container, Type stateType, Type instanceType, Type implementationType, object key = null)
        {
            var binder = container.Resolve<IBinder>();
            return binder.Bind(container, stateType, instanceType, implementationType, key);
        }
    }
}
