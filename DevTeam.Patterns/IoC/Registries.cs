namespace DevTeam.Patterns.IoC
{
    using System;

    public static class Registries
    {
        public static IRegistrationDescription<TImplementation> Register<TImplementation>(this IContainer container, WellknownLifetime lifetime = WellknownLifetime.Transient)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return new RegistrationDescription<TImplementation>(container, lifetime);
        }

        public static IRegistrationDescription<object> Register(this IContainer container, Type implementationType, WellknownLifetime lifetime = WellknownLifetime.Transient)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return new RegistrationDescription<object>(container, implementationType, lifetime);
        }

        public static IRegistrationDescription<TImplementation> FindingBy<TImplementation>(this IRegistrationDescription<TImplementation> registration, WellknownComparer comparer)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            registration.Comparer = comparer;
            return registration;
        }

        public static IRegistrationDescription<TImplementation> InScope<TImplementation>(this IRegistrationDescription<TImplementation> registration, WellknownScope scope)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            registration.Scope = scope;
            return registration;
        }

        public static IRegistrationDescription<TImplementation> InRange<TImplementation>(this IRegistrationDescription<TImplementation> registration, WellknownContractRange contgractRange)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            registration.ContractRange = contgractRange;
            return registration;
        }

        public static IRegistration As<TState, T>(this IRegistrationDescription<T> registration, object key = null)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            return registration.As(typeof(TState), typeof(T), key);
        }

        public static IRegistration As<T>(this IRegistrationDescription<T> registration, object key = null)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            return registration.As<EmptyState, T>(key);
        }

        public static IRegistration As<T>(this IRegistrationDescription<T> registration, Type contractType, object key = null)
        {
            return registration.As(typeof(EmptyState), contractType, key);
        }

        public static IRegistration As<T>(this IRegistrationDescription<T> registration, Type stateType, Type contractType, object key = null)
        {
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));

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

            if (registration.ContractRange != WellknownContractRange.Contract)
            {
                container = container.Using<IContractRange>(registration.ContractRange);
            }

            return container.Register(stateType, contractType, registration.ImplementationType, key);
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

        public interface IRegistrationDescription<out T>
        {
            Type ImplementationType { get; }

            IContainer Container { get; }

            WellknownLifetime Lifetime { get; set; }

            WellknownComparer Comparer { get; set; }

            WellknownScope Scope { get; set; }

            WellknownContractRange ContractRange { get; set; }
        }

        private class RegistrationDescription<TImplementation>: IRegistrationDescription<TImplementation>
        {
            public RegistrationDescription(IContainer container, Type implementationType, WellknownLifetime lifetime)
            {
                if (container == null) throw new ArgumentNullException(nameof(container));

                ImplementationType = implementationType;
                Container = container;
                Lifetime = lifetime;
                Comparer = WellknownComparer.FullCompliance;
                Scope = WellknownScope.Public;
            }

            public RegistrationDescription(IContainer container, WellknownLifetime lifetime)
                :this(container, typeof(TImplementation), lifetime)
            {
            }

            public Type ImplementationType { get; }

            public IContainer Container { get; }

            public WellknownLifetime Lifetime { get; set; }

            public WellknownComparer Comparer { get; set; }

            public WellknownScope Scope { get; set; }

            public WellknownContractRange ContractRange { get; set; }
        }
    }
}