namespace DevTeam.Patterns.IoC
{
    using System;

    internal class IoCContainerConfiguration: IConfiguration
    {
        private static readonly Lazy<ILifetime> TransientLifetime = new Lazy<ILifetime>(() => new TransientLifetime());
        private static readonly Lazy<ILifetime> SingletoneLifetime = new Lazy<ILifetime>(() => new SingletoneLifetime(new ControlledLifetime()));
        private static readonly Lazy<ILifetime> ControlledLifetime = new Lazy<ILifetime>(() => new ControlledLifetime());

        public IContainer Apply(IContainer container)
        {
            container.Register(() => SingletoneLifetime.Value, WellknownLifetime.Singletone);
            container.Register(() => TransientLifetime.Value, WellknownLifetime.Transient);
            container.Register(() => ControlledLifetime.Value, WellknownLifetime.Controlled);

            container.Using<ILifetime>(WellknownLifetime.Controlled).Register<ContainerInfo, IContainer>(childContainerInfo => new Container(childContainerInfo));

            return container;
        }
    }
}
