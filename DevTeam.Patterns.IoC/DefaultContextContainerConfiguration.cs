namespace DevTeam.Patterns.IoC
{
    using System;

    internal class DefaultContextContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            var defaultFactory = new Lazy<ILifetime>(() => new Transient());
            container.Register(() => defaultFactory.Value);
            return container;
        }
    }
}
