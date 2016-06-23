namespace Echo
{
    using System;

    using DevTeam.Patterns.Dispose;
    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.IoC;

    internal class EchoConfiguration : IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            var disposable = new CompositeDisposable();

            disposable.Add(new EventAggregatorContainerConfiguration().Apply(container));

            // Register EchoService
            disposable.Add(container.Register<string, IEchoService>(
                id => new EchoService(id, container.Resolve<IEventAggregator>())));

            // Register ConsoleEchoPublisher as Singletone
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEchoPublisher>(
                () => new ConsoleEchoPublisher()));

            // Register ConsoleEchoRequestSource as Singletone
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEchoRequestSource>(
                () => new ConsoleEchoRequestSource()));

            return disposable;
        }
    }
}
