namespace DevTeam.TestTool.dotNet
{
    using System;

    using Patterns.Dispose;
    using Engine.Contracts;

    using Patterns.IoC;

    public class DotNetContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IReflection>(() => new Reflection()));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IOutput>(() => new Console(), nameof(Console)));

            return disposable;
        }
    }
}
