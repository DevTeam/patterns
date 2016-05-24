namespace DevTeam.TestTool.dotNet
{
    using System;

    using Patterns.IoC;
    using Engine.Contracts;

    public class DotNetContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container = container.Resolve<IContainer>(typeof(DotNetContainerConfiguration).Name);

            var reflection = new Lazy<IReflection>(() => new Reflection());

            container
                .Register(() => reflection.Value);

            return container;
        }
    }
}
