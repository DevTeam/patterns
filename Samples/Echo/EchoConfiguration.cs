namespace Echo
{
    using System;
    using System.Collections.Generic;

    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.IoC;

    internal class EchoConfiguration : IConfiguration
    {        
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new EventAggregatorContainerConfiguration();
        }

        public IEnumerable<IDisposable> CreateRegistrations(IContainer container)
        {
            // Register echo request
            yield return container.Register<string, IEchoRequest>(
                message => new EchoRequest(message));

            // Register echo
            yield return container.Register<string, IEcho>(
                message => new Echo(message));

            // Register EchoService
            yield return container.Register<string, IEchoService>(
                id => new EchoService(id, container.Resolve<IEventAggregator>(), container.Resolver<string, IEcho>()));

            // Register ConsoleEchoPublisher as Singleton
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IEchoPublisher>(
                () => new ConsoleEchoPublisher());

            // Register ConsoleEchoRequestSource as Singleton
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IEchoRequestSource>(
                () => new ConsoleEchoRequestSource(container.Resolver<string, IEchoRequest>()));
        }
    }
}
