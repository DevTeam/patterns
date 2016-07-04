namespace DevTeam.Patterns.IoC
{
    using System;

    using Dispose;

    internal class IoCContainerConfiguration: IConfiguration
    {
        internal static readonly Lazy<ILifetime> TransientLifetime = new Lazy<ILifetime>(() => new TransientLifetime());
        private static readonly Lazy<ILifetime> SingletoneLifetime = new Lazy<ILifetime>(() => new SingletoneLifetime(new ControlledLifetime()));
        private static readonly Lazy<ILifetime> ControlledLifetime = new Lazy<ILifetime>(() => new ControlledLifetime());
        internal static readonly Lazy<IRegistryKeyComparer> RootContainerRegestryKeyComparer = new Lazy<IRegistryKeyComparer>(() => new RootContainerRegestryKeyComparer());
        private static readonly Lazy<IRegistryKeyComparer> NamePatternRegestryKeyComparer = new Lazy<IRegistryKeyComparer>(() => new NamePatternRegestryKeyComparer());

        public IDisposable Apply(IContainer container)
        {
            var disposable = new CompositeDisposable();

            // Wellknown lifetimes
            disposable.Add(container.Register(() => TransientLifetime.Value, WellknownLifetime.Transient));
            disposable.Add(container.Register(() => SingletoneLifetime.Value, WellknownLifetime.Singletone));
            disposable.Add(container.Register(() => ControlledLifetime.Value, WellknownLifetime.Controlled));

            // Wellknown regestry key comparer
            disposable.Add(container.Register(() => NamePatternRegestryKeyComparer.Value, WellknownRegestryKeyComparer.NamePattern));

            // Child container
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Controlled).Register(typeof(EmptyState), typeof(IContainer), ctx => new Container(new ContainerDescription(ctx.Container, ctx.RegestryKey.Name))));

            // Resolvers
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(typeof(EmptyState), typeof(IResolver<>),
                ctx =>
                    {
                        var resolverType = typeof(Resolver<>).MakeGenericType(ctx.ResolvingInstanceType.GenericTypeArguments[0]);
                        return Activator.CreateInstance(resolverType, container);
                    }));

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(typeof(EmptyState), typeof(IResolver<,>),
                ctx =>
                {
                    var resolverType = typeof(Resolver<,>).MakeGenericType(ctx.ResolvingInstanceType.GenericTypeArguments[0], ctx.ResolvingInstanceType.GenericTypeArguments[1]);
                    return Activator.CreateInstance(resolverType, container);
                }));

            return disposable;
        }
    }
}
