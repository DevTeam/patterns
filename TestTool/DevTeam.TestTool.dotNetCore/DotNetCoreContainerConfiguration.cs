namespace DevTeam.TestTool.dotNetCore
{
    using System;

    using Patterns.Dispose;
    using Patterns.IoC;
    using Engine.Contracts;

    public class DotNetCoreContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IOutput>(() => new Console(), nameof(Console)));

            return disposable;
        }
    }
}
