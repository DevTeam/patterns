namespace DevTeam.Patterns.IoC
{
    using System;

    public static class Registries
    {
        public static Registration Register<TImplementation>(this IContainer container, WellknownLifetime lifetime = WellknownLifetime.Transient)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return container.Register(typeof(TImplementation), lifetime);
        }

        public static Registration Register(this IContainer container, Type implementationType, WellknownLifetime lifetime = WellknownLifetime.Transient)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            return new Registration(implementationType, container, lifetime);
        }

        public static Registration FindingBy(this Registration registration, WellknownComparer comparer)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            registration.Comparer = comparer;
            return registration;
        }

        public static Registration InScope(this Registration registration, WellknownScope scope)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            registration.Scope = scope;
            return registration;
        }

        public static IRegistration As<TState, T>(this Registration registration, object key = null)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            return registration.As(typeof(TState), typeof(T), key);
        }

        public static IRegistration As(this Registration registration, Type stateType, Type contractType, object key = null)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            var container = registration.Container;

            if (registration.Lifetime != WellknownLifetime.Transient)
            {
                container = container.Using<ILifetime>(registration.Lifetime);
            }

            if (registration.Comparer != WellknownComparer.FullCompliance)
            {
                container = container.Using<IComparer>(registration.Comparer);
            }

            if (registration.Scope != WellknownScope.Public)
            {
                container = container.Using<IScope>(registration.Scope);
            }

            return container.Register(stateType, contractType, registration.ImplementationType, key);
        }

        public static IRegistration As(this Registration registration, Type contractType, object key = null)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            return registration.As(typeof(EmptyState), contractType, key);
        }

        public static IRegistration As<T>(this Registration registration, object key = null)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            return registration.As<EmptyState, T>(key);
        }

        public static IRegistration Register(this IContainer container, Type stateType, Type contractType, Type implementationType, object key = null)
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

        public static IRegistration Register<TState, T>(this IRegistry registry, Func<TState, T> factoryMethod, object key = null)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            return registry.Register(typeof(TState), typeof(T), ctx => factoryMethod((TState)ctx.State), key);
        }

        public static IRegistration Register<T>(this IRegistry registry, Func<T> factoryMethod, object key = null)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            return registry.Register(new Func<EmptyState, T>(ignoredArg => factoryMethod()), key);
        }

        public static IContainer Using<TContext>(this IContainer container, object contextKey = null)
            where TContext : IContext
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return container.Using(container.Resolve<TContext>(contextKey), typeof(TContext));
        }

        public static IContainer Using(this IContainer container, Type contextType, object contextKey = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (contextType == null) throw new ArgumentNullException(nameof(contextType));

            var context = (IContext)container.Resolve(typeof(EmptyState), contextType, contextKey);
            return (IContainer)container.Resolve(typeof(ContextContainerState), typeof(IContextContainer), new ContextContainerState(container, context, contextType));
        }

        public static IContainer Using(this IContainer container, IContext context, Type contextType)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (contextType == null) throw new ArgumentNullException(nameof(contextType));

            return (IContainer)container.Resolve(typeof(ContextContainerState), typeof(IContextContainer), new ContextContainerState(container, context, contextType));
        }

        public class Registration
        {
            public Registration(Type implementationType, IContainer container, WellknownLifetime lifetime)
            {
                if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));
                if (container == null) throw new ArgumentNullException(nameof(container));

                ImplementationType = implementationType;
                Container = container;
                Lifetime = lifetime;
                Comparer = WellknownComparer.FullCompliance;
                Scope = WellknownScope.Public;
            }

            internal Type ImplementationType { get; }

            internal IContainer Container { get; }

            internal WellknownLifetime Lifetime { get; }

            internal WellknownComparer Comparer { get; set; }

            public WellknownScope Scope { get; set; }
        }
    }
}