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

            disposable.Add(container.Register(() => TransientLifetime.Value, WellknownLifetime.Transient));
            disposable.Add(container.Register(() => SingletoneLifetime.Value, WellknownLifetime.Singletone));
            disposable.Add(container.Register(() => ControlledLifetime.Value, WellknownLifetime.Controlled));

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Controlled).Register<ContainerInfo, IContainer>(childContainerInfo => new Container(childContainerInfo)));

            return disposable;
        }
    }
}
