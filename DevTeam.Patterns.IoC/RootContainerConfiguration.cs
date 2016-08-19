namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class RootContainerConfiguration : IConfiguration
    {
        internal static readonly ILifetime TransientLifetime = new TransientLifetime();

        internal static readonly IComparer FullComplianceComparer = new FullComplianceComparer();
        private static readonly IComparer PatternComparer = new PatternKeyComparer();
        private static readonly IComparer AnyStateTypeAndKey = new AnyStateTypeAndKeyComparer();
        private static readonly IComparer AnyComparer = new AnyKeyComparer();

        internal static readonly IContractRange ContractRange = new ContractRange();
        private static readonly IContractRange ImplementationContractRange = new ImplementationContractRange(ContractRange);
        private static readonly IContractRange InheritanceContractRange = new InheritanceContractRange(ImplementationContractRange);

        internal static readonly IBinder Binder = new Binder();
        internal static readonly IFactory Factory = new ExpressionFactory();

        internal static readonly IScope PublicScope = new PublicScope();

        internal static readonly IConfiguration Shared = new RootContainerConfiguration();

        private readonly ILifetime _controlledLifetime = new ControlledLifetime();
        private readonly ILifetime _singletonLifetime = new SingletonLifetime(TransientLifetime);
        private readonly Lazy<ILifetime> _controlledSingletonLifetime = new Lazy<ILifetime>(() => new SingletonLifetime(new ControlledLifetime()));
        private readonly Lazy<ILifetime> _perContainerLifetime = new Lazy<ILifetime>(() => new PerContainerLifetime(TransientLifetime));
        private readonly Lazy<ILifetime> _controlledPerContainerLifetime = new Lazy<ILifetime>(() => new PerContainerLifetime(new ControlledLifetime()));
        private readonly Lazy<ILifetime> _perResolveLifetime = new Lazy<ILifetime>(() => new PerResolveLifetime(TransientLifetime));
        private readonly Lazy<ILifetime> _controlledPerResolveLifetime = new Lazy<ILifetime>(() => new PerResolveLifetime(new ControlledLifetime()));
        private readonly Lazy<ILifetime> _perThreadLifetime = new Lazy<ILifetime>(() => new PerThreadLifetime(TransientLifetime));
        private readonly Lazy<ILifetime> _controlledPerThreadLifetime = new Lazy<ILifetime>(() => new PerThreadLifetime(new ControlledLifetime()));

        private RootContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            // Wellknown lifetimes
            yield return container.Register(() => TransientLifetime, WellknownLifetime.Transient);
            yield return container.Register(() => _controlledLifetime, WellknownLifetime.Controlled);
            yield return container.Register(() => _singletonLifetime, WellknownLifetime.Singleton);
            yield return container.Register(() => _controlledSingletonLifetime.Value, WellknownLifetime.ControlledSingleton);
            yield return container.Register(() => _perContainerLifetime.Value, WellknownLifetime.PerContainer);
            yield return container.Register(() => _controlledPerContainerLifetime.Value, WellknownLifetime.ControlledPerContainer);
            yield return container.Register(() => _perResolveLifetime.Value, WellknownLifetime.PerResolveLifetime);
            yield return container.Register(() => _controlledPerResolveLifetime.Value, WellknownLifetime.ControlledPerResolveLifetime);
            yield return container.Register(() => _perThreadLifetime.Value, WellknownLifetime.PerThreadLifetime);
            yield return container.Register(() => _controlledPerThreadLifetime.Value, WellknownLifetime.ControlledPerThreadLifetime);

            // Wellknown comparers
            yield return container.Register(() => FullComplianceComparer, WellknownComparer.FullCompliance);
            yield return container.Register(() => PatternComparer, WellknownComparer.PatternKey);
            yield return container.Register(() => AnyStateTypeAndKey, WellknownComparer.AnyStateTypeAndKey);
            yield return container.Register(() => AnyComparer, WellknownComparer.AnyKey);

            // Wellknown contract ranges
            yield return container.Register(() => ContractRange, WellknownContractRange.Contract);
            yield return container.Register(() => ImplementationContractRange, WellknownContractRange.Implementation);
            yield return container.Register(() => InheritanceContractRange, WellknownContractRange.Inheritance);

            // Context container
            yield return container.Register(typeof(ContextContainerState), typeof(IContextContainer),
                ctx =>
                {
                    var state = (ContextContainerState)ctx.State;
                    var resolverType = typeof(ContextContainer<>).MakeGenericType(state.ContextType);
                    var ctor = resolverType.GetTypeInfo().DeclaredConstructors.Single(i => i.GetParameters().Length == 1 && i.GetParameters()[0].ParameterType == typeof(ContextContainerState));
                    return Factory.Create(ctor, ctx.State);
                });

            // Child container
            yield return container
                .Using<IComparer>(WellknownComparer.AnyStateTypeAndKey)
                .Using<ILifetime>(WellknownLifetime.Controlled)
                .Register(
                    typeof(EmptyState),
                    typeof(IContainer),
                    ctx => new Container(new ContainerDescription(ctx.ResolveContainer, ctx.Registration.Key)));

            // Resolvers
            yield return container
                .Using<ILifetime>(WellknownLifetime.Singleton)
                .Using<IComparer>(WellknownComparer.AnyKey)
                .Register(typeof(EmptyState), typeof(IResolver<>),
                    ctx =>
                    {
                        var resolverType = typeof(Resolver<>).MakeGenericType(ctx.ResolvingContractType.GenericTypeArguments[0]);
                        var ctor = resolverType.GetTypeInfo().DeclaredConstructors.Single(i => i.GetParameters().Length == 2 && i.GetParameters()[0].ParameterType == typeof(IResolver) && i.GetParameters()[1].ParameterType == typeof(object));
                        return Factory.Create(ctor, ctx.ResolveContainer, ctx.Registration.Key);
                    });

            yield return container
                .Using<ILifetime>(WellknownLifetime.Singleton)
                .Using<IComparer>(WellknownComparer.AnyKey)
                .Register(typeof(EmptyState), typeof(IResolver<,>),
                    ctx =>
                    {
                        var resolverType = typeof(Resolver<,>).MakeGenericType(ctx.ResolvingContractType.GenericTypeArguments[0], ctx.ResolvingContractType.GenericTypeArguments[1]);
                        var ctor = resolverType.GetTypeInfo().DeclaredConstructors.Single(i => i.GetParameters().Length == 2 && i.GetParameters()[0].ParameterType == typeof(IResolver) && i.GetParameters()[1].ParameterType == typeof(object));
                        return Factory.Create(ctor, ctx.ResolveContainer, ctx.Registration.Key);
                    });

            // Resolve All as IEnumerable
            yield return container
                .Using<IComparer>(WellknownComparer.AnyStateTypeAndKey)
                .Register(
                    typeof(EmptyState),
                    typeof(IEnumerable<>),
                    ctx =>
                    {
                        var enumItemType = ctx.ResolvingContractType.GenericTypeArguments[0];
                        var enumType = typeof(Enumerable<>).MakeGenericType(enumItemType);
                        var source =
                            from key in ctx.ResolveContainer.GetRegistrations()
                            where key.ContractType == enumItemType && key.StateType == ctx.Registration.StateType
                            select ctx.ResolveContainer.Resolve(key.StateType, enumItemType, ctx.State, key.Key);
                        var ctor = enumType.GetTypeInfo().DeclaredConstructors.Single(i => i.GetParameters().Length == 1);
                        return Factory.Create(ctor, source);
                    });

            yield return container
                .Using<IComparer>(WellknownComparer.AnyKey)
                .Register(
                    typeof(StateSelector),
                    typeof(IEnumerable<>),
                    ctx =>
                    {
                        var enumItemType = ctx.ResolvingContractType.GenericTypeArguments[0];
                        var enumType = typeof(Enumerable<>).MakeGenericType(enumItemType);
                        var source =
                            from key in ctx.ResolveContainer.GetRegistrations()
                            where key.ContractType == enumItemType
                            let state = ((StateSelector)ctx.State)(ctx)
                            select ctx.ResolveContainer.Resolve(key.StateType, enumItemType, state, key.Key);
                        var ctor = enumType.GetTypeInfo().DeclaredConstructors.Single(i => i.GetParameters().Length == 1);
                        return Factory.Create(ctor, source);
                    });

            // Scopes
            yield return container.Register(typeof(EmptyState), typeof(IScope), ctx => PublicScope, WellknownScope.Public);
            yield return container.Register(typeof(EmptyState), typeof(IScope), ctx => new InternalScope(ctx.ResolveContainer), WellknownScope.Internal);
            yield return container.Register(typeof(EmptyState), typeof(IScope), ctx => new GlobalScope(ctx.ResolveContainer), WellknownScope.Global);

            yield return container
                .Using<ILifetime>(WellknownLifetime.Singleton)
                .Register(typeof(EmptyState), typeof(IRegistrationFactory), ctx => new RegistrationFactory());
        }

        private class Enumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerable<object> _source;

            public Enumerable(IEnumerable<object> source)
            {
                if (source == null) throw new ArgumentNullException(nameof(source));

                _source = source;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new Enumerator<T>(_source.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class Enumerator<T> : IEnumerator<T>
        {
            private readonly IEnumerator<object> _source;

            public Enumerator(IEnumerator<object> source)
            {
                if (source == null) throw new ArgumentNullException(nameof(source));

                _source = source;
            }

            public T Current => (T)_source.Current;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                return _source.MoveNext();
            }

            public void Reset()
            {
                _source.Reset();
            }

            public void Dispose()
            {
                _source.Dispose();
            }
        }
    }
}