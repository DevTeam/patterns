namespace DevTeam.Patterns.IoC
{
    using System;

    using Dispose;

    internal class IoCContainerConfiguration: IConfiguration
    {
        internal static readonly Lazy<ILifetime> TransientLifetime = new Lazy<ILifetime>(() => new TransientLifetime());
        private static readonly Lazy<ILifetime> SingletoneLifetime = new Lazy<ILifetime>(() => new SingletoneLifetime(new ControlledLifetime()));
        private static readonly Lazy<ILifetime> ControlledLifetime = new Lazy<ILifetime>(() => new ControlledLifetime());

        public IDisposable Apply(IContainer container)
        {
            var disposable = new CompositeDisposable();

            // Wellknown lifetimes
            disposable.Add(container.Register(() => TransientLifetime.Value, WellknownLifetime.Transient));
            disposable.Add(container.Register(() => SingletoneLifetime.Value, WellknownLifetime.Singletone));
            disposable.Add(container.Register(() => ControlledLifetime.Value, WellknownLifetime.Controlled));

            // Child container
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Controlled).Register<ContainerDescription, IContainer>(containerDescription => new Container(containerDescription)));

            // Resolvers
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(typeof(EmptyState), typeof(IResolver<>),
                (type, emptyState) =>
                    {
                        var resolverType = typeof(Resolver<>).MakeGenericType(type.GenericTypeArguments[0]);
                        return Activator.CreateInstance(resolverType, container);
                    }));

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(typeof(EmptyState), typeof(IResolver<,>),
                (type, emptyState) =>
                {
                    var resolverType = typeof(Resolver<,>).MakeGenericType(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
                    return Activator.CreateInstance(resolverType, container);
                }));

            return disposable;
        }
    }
}
