namespace DevTeam.TestTool.dotNet
{
    using System;

    using DevTeam.TestTool.Engine.Contracts;

    using Patterns.IoC;

    public class DotNetContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container = container.Resolve<IContainer>(nameof(DotNetContainerConfiguration));
            
            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register<IReflection>(() => new Reflection());

            return container;
        }
    }
}
