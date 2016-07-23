using System.Collections;
using System.Linq;

namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class ContainerConfiguration: IConfiguration
    {
        internal static readonly IConfiguration Shared = new ContainerConfiguration();
        internal static readonly Lazy<ILifetime> TransientLifetime = new Lazy<ILifetime>(() => new TransientLifetime());        
        private static readonly Lazy<ILifetime> ControlledLifetime = new Lazy<ILifetime>(() => new ControlledLifetime());
        private static readonly Lazy<ILifetime> SingletonLifetime = new Lazy<ILifetime>(() => new SingletonLifetime(new TransientLifetime()));
        private static readonly Lazy<ILifetime> ControlledSingletonLifetime = new Lazy<ILifetime>(() => new SingletonLifetime(new ControlledLifetime()));
        private static readonly Lazy<ILifetime> PerContainerLifetime = new Lazy<ILifetime>(() => new PerContainerLifetime(new TransientLifetime()));
        private static readonly Lazy<ILifetime> ControlledPerContainerLifetime = new Lazy<ILifetime>(() => new PerContainerLifetime(new ControlledLifetime()));
        private static readonly Lazy<ILifetime> PerResolveLifetime = new Lazy<ILifetime>(() => new PerResolveLifetimeLifetime(new TransientLifetime()));
        private static readonly Lazy<ILifetime> ControlledPerResolveLifetime = new Lazy<ILifetime>(() => new PerResolveLifetimeLifetime(new ControlledLifetime()));
        private static readonly Lazy<ILifetime> PerThreadLifetime = new Lazy<ILifetime>(() => new PerThreadLifetime(new TransientLifetime()));
        private static readonly Lazy<ILifetime> ControlledPerThreadLifetime = new Lazy<ILifetime>(() => new PerThreadLifetime(new ControlledLifetime()));

        internal static readonly Lazy<IRegistrationComparer> RootContainerRegestryKeyComparer = new Lazy<IRegistrationComparer>(() => new RootContainerRegistrationComparer());
        private static readonly Lazy<IRegistrationComparer> PatternRegistrationComparer = new Lazy<IRegistrationComparer>(() => new PatternKeyRegistrationComparer());
        private static readonly Lazy<IRegistrationComparer> AnyStateTypeAndKey = new Lazy<IRegistrationComparer>(() => new AnyStateTypeAndKeyRegistrationComparer());
        private static readonly Lazy<IRegistrationComparer> AnyRegistrationComparer = new Lazy<IRegistrationComparer>(() => new AnyKeyRegistrationComparer());

        internal static readonly Lazy<IBinder> ReflectionBinder = new Lazy<IBinder>(() => new ReflectionBinder());

        private ContainerConfiguration()
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
            yield return container.Register(() => TransientLifetime.Value, WellknownLifetime.Transient);
            yield return container.Register(() => ControlledLifetime.Value, WellknownLifetime.Controlled);
            yield return container.Register(() => SingletonLifetime.Value, WellknownLifetime.Singleton);
            yield return container.Register(() => ControlledSingletonLifetime.Value, WellknownLifetime.ControlledSingleton);
            yield return container.Register(() => PerContainerLifetime.Value, WellknownLifetime.PerContainer);
            yield return container.Register(() => ControlledPerContainerLifetime.Value, WellknownLifetime.ControlledPerContainer);
            yield return container.Register(() => PerResolveLifetime.Value, WellknownLifetime.PerResolveLifetime);
            yield return container.Register(() => ControlledPerResolveLifetime.Value, WellknownLifetime.ControlledPerResolveLifetime);
            yield return container.Register(() => PerThreadLifetime.Value, WellknownLifetime.PerThreadLifetime);
            yield return container.Register(() => ControlledPerThreadLifetime.Value, WellknownLifetime.ControlledPerThreadLifetime);

            // Wellknown registration comparers
            yield return container.Register(() => PatternRegistrationComparer.Value, WellknownRegistrationComparer.PatternKey);
            yield return container.Register(() => AnyStateTypeAndKey.Value, WellknownRegistrationComparer.AnyStateTypeAndKey);
            yield return container.Register(() => AnyRegistrationComparer.Value, WellknownRegistrationComparer.AnyKey);

            // Wellknown binders
            yield return container.Register(() => ReflectionBinder.Value);
            yield return container.Register(() => ReflectionBinder.Value, WellknownBinder.Reflection);

            // Context container
            yield return container.Register(typeof(ContextContainerState), typeof(IContextContainer<>),
                ctx =>
                {
                    var resolverType = typeof(ContextContainer<>).MakeGenericType(ctx.ResolvingInstanceType.GenericTypeArguments[0]);
                    return Activator.CreateInstance(resolverType, ctx.State);
                });

            // Child container
            yield return container.Using<ILifetime>(WellknownLifetime.Controlled).Register(typeof(EmptyState), typeof(IContainer), ctx => new Container(new ContainerDescription(ctx.ResolvingContainer, ctx.Registration.Key)));
            yield return container.Using<ILifetime>(WellknownLifetime.Controlled).Register(typeof(object), typeof(IContainer), ctx => new Container(new ContainerDescription(ctx.ResolvingContainer, ctx.State)));

            // Resolvers
            yield return container
                .Using<ILifetime>(WellknownLifetime.Singleton)
                .Using<IRegistrationComparer>(WellknownRegistrationComparer.AnyKey)
                .Register(typeof(EmptyState), typeof(IResolver<>),
                ctx =>
                    {
                        var resolverType = typeof(Resolver<>).MakeGenericType(ctx.ResolvingInstanceType.GenericTypeArguments[0]);
                        return Activator.CreateInstance(resolverType, container, ctx.Registration.Key);
                    });

            yield return container
                .Using<ILifetime>(WellknownLifetime.Singleton)
                .Using<IRegistrationComparer>(WellknownRegistrationComparer.AnyKey)
                .Register(typeof(EmptyState), typeof(IResolver<,>),
                ctx =>
                    {
                        var resolverType = typeof(Resolver<,>).MakeGenericType(ctx.ResolvingInstanceType.GenericTypeArguments[0], ctx.ResolvingInstanceType.GenericTypeArguments[1]);
                        return Activator.CreateInstance(resolverType, container, ctx.Registration.Key);
                    });

            // Default configuration equality comparer
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register(typeof(EmptyState), typeof(IEqualityComparer<IConfiguration>), ctx => new ConfigurationEqualityComparer());

            // Resolve All as IEnumerable
            yield return container
                .Using<IRegistrationComparer>(WellknownRegistrationComparer.AnyStateTypeAndKey)
                .Register(
                typeof(EmptyState),
                typeof(IEnumerable<>),
                ctx =>
                {
                    var enumItemType = ctx.ResolvingInstanceType.GenericTypeArguments[0];
                    var enumType = typeof(Enumerable<>).MakeGenericType(enumItemType);
                    var source =
                        from key in ctx.ResolvingContainer.Registrations
                        where key.InstanceType == enumItemType && key.StateType == ctx.Registration.StateType
                        select ctx.ResolvingContainer.Resolve(key.StateType, enumItemType, ctx.State, key.Key);
                    return Activator.CreateInstance(enumType, source);
                },
                null);
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

        private class Enumerator<T>: IEnumerator<T>
        {
            private readonly IEnumerator<object> _source;

            public Enumerator(IEnumerator<object> source)
            {
                if (source == null) throw new ArgumentNullException(nameof(source));

                _source = source;
            }

            public T Current => (T) _source.Current;

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