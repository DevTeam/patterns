namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

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

        public static IRegistration As<TState, T>(this IRegistrationDescription<T> description, object key = null)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));

            return description.As(typeof(TState), typeof(T), key);
        }

        public static IRegistration As<T>(this IRegistrationDescription<T> description, object key = null)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));

            return description.As<EmptyState, T>(key);
        }

        public static IRegistration As<T>(this IRegistrationDescription<T> description, Type contractType, object key = null)
        {
            return description.As(typeof(EmptyState), contractType, key);
        }

        public static IRegistration As<T>(this IRegistrationDescription<T> description, Type stateType, Type contractType, object key = null)
        {
            var registrationFactory = description.Container.Resolve<IRegistrationFactory>();
            return registrationFactory.Create(description, stateType, contractType, key);
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

        public interface IRegistrationDescription<out TImplementation>: IRegistrationDescription
        {
            IRegistrationDescription<TImplementation> FindingBy(WellknownComparer comparer);

            IRegistrationDescription<TImplementation> InScope(WellknownScope scope);

            IRegistrationDescription<TImplementation> InRange(WellknownContractRange contractRange);

            IRegistrationDescription<TImplementation> As(Type contractType);

            IRegistrationDescription<TImplementation> As<T>();
        }

        private class RegistrationDescription<TImplementation>: IRegistrationDescription<TImplementation>
        {
            private readonly List<Type> _additionalContracts = new List<Type>();

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

            public WellknownLifetime Lifetime { get; }

            public WellknownComparer Comparer { get; private set; }

            public WellknownScope Scope { get; private set; }

            public WellknownContractRange ContractRange { get; private set; }

            public IEnumerable<Type> AdditionalContracts => _additionalContracts;

            public IRegistrationDescription<TImplementation> FindingBy(WellknownComparer comparer = WellknownComparer.FullCompliance)
            {
                Comparer = comparer;
                return this;
            }

            public IRegistrationDescription<TImplementation> InScope(WellknownScope scope)
            {
                Scope = scope;
                return this;
            }

            public IRegistrationDescription<TImplementation> InRange(WellknownContractRange contractRange)
            {
                ContractRange = contractRange;
                return this;
            }

            public IRegistrationDescription<TImplementation> As(Type contractType)
            {
                _additionalContracts.Add(contractType);
                return this;
            }

            public IRegistrationDescription<TImplementation> As<T>()
            {
                _additionalContracts.Add(typeof(T));
                return this;
            }
        }
    }
}