namespace DevTeam.Patterns.IoC
{
    using System;

    public class IoCContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            container = container.Resolve<IContainer>(nameof(IoCContainerConfiguration));

            var transient = new Lazy<ILifetime>(() => new Transient());
            var singletone = new Lazy<ILifetime>(() => new Singletone());

            container.Register(() => transient.Value, WellknownLifetime.Transient);
            container.Register(() => singletone.Value, WellknownLifetime.Singletone);

            return container;
        }
    }
}
