namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class IoCContainerConfiguration: IConfiguration
    {
        internal static readonly IConfiguration Shared = new IoCContainerConfiguration();
        internal static readonly Lazy<ILifetime> TransientLifetime = new Lazy<ILifetime>(() => new TransientLifetime());
        private static readonly Lazy<ILifetime> SingletoneLifetime = new Lazy<ILifetime>(() => new SingletonLifetime(new ControlledLifetime()));
        private static readonly Lazy<ILifetime> ControlledLifetime = new Lazy<ILifetime>(() => new ControlledLifetime());
        internal static readonly Lazy<IRegistrationComparer> RootContainerRegestryKeyComparer = new Lazy<IRegistrationComparer>(() => new RootContainerRegistrationComparer());
        private static readonly Lazy<IRegistrationComparer> NamePatternRegestryKeyComparer = new Lazy<IRegistrationComparer>(() => new NamePatternRegistrationComparer());

        private IoCContainerConfiguration()
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
            yield return container.Register(() => SingletoneLifetime.Value, WellknownLifetime.Singleton);
            yield return container.Register(() => ControlledLifetime.Value, WellknownLifetime.Controlled);

            // Wellknown registration comparer
            yield return container.Register(() => NamePatternRegestryKeyComparer.Value, WellknownRegistrationComparer.NamePattern);

            // Child container
            yield return container.Using<ILifetime>(WellknownLifetime.Controlled).Register(typeof(EmptyState), typeof(IContainer), ctx => new Container(new ContainerDescription(ctx.Container, ctx.Registration.Name)));

            // Resolvers
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register(typeof(EmptyState), typeof(IResolver<>),
                ctx =>
                    {
                        var resolverType = typeof(Resolver<>).MakeGenericType(ctx.ResolvingInstanceType.GenericTypeArguments[0]);
                        return Activator.CreateInstance(resolverType, container);
                    });

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register(typeof(EmptyState), typeof(IResolver<,>),
                ctx =>
                {
                    var resolverType = typeof(Resolver<,>).MakeGenericType(ctx.ResolvingInstanceType.GenericTypeArguments[0], ctx.ResolvingInstanceType.GenericTypeArguments[1]);
                    return Activator.CreateInstance(resolverType, container);
                });

            // Default configuration equality comparer
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register(typeof(EmptyState), typeof(IEqualityComparer<IConfiguration>), ctx => new ConfigurationEqualityComparer());
        }        
    }
}