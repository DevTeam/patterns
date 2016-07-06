namespace Echo
{
    using System;
    using System.Collections.Generic;

    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.IoC;

    internal class EchoConfiguration : IConfiguration
    {
        public static readonly IConfiguration Shared = new EchoConfiguration();

        private EchoConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return EventAggregatorContainerConfiguration.Shared;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
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

            // Register ConsoleEchoPublisher as Singletone
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEchoPublisher>(
                () => new ConsoleEchoPublisher());

            // Register ConsoleEchoRequestSource as Singletone
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEchoRequestSource>(
                () => new ConsoleEchoRequestSource(container.Resolver<string, IEchoRequest>()));
        }
    }
}
