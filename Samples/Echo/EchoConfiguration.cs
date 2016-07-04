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

            // Register echo request
            disposable.Add(container.Register<string, IEchoRequest>(
                message => new EchoRequest(message)));

            // Register echo
            disposable.Add(container.Register<string, IEcho>(
                message => new Echo(message)));

            // Register EchoService
            disposable.Add(container.Register<string, IEchoService>(
                id => new EchoService(id, container.Resolve<IEventAggregator>(), container.Resolver<string, IEcho>())));

            // Register ConsoleEchoPublisher as Singletone
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEchoPublisher>(
                () => new ConsoleEchoPublisher()));

            // Register ConsoleEchoRequestSource as Singletone
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEchoRequestSource>(
                () => new ConsoleEchoRequestSource(container.Resolver<string, IEchoRequest>())));

            return disposable;
        }
    }
}
