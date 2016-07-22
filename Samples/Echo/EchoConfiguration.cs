namespace Echo
{
    using System.Collections.Generic;

    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.IoC;
    using DevTeam.Patterns.Reactive;
    using DevTeam.Platform.System;

    /// <inheritdoc/>
    internal class EchoConfiguration : IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new SystemContainerConfiguration();
            yield return new EventAggregatorContainerConfiguration();
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            // Register echo request
            yield return container.Register<string, IEchoRequest>(
                message => new EchoRequest(message));

            // Register echo
            yield return container.Register<string, IEcho>(
                message => new Echo(message));

            // Register EchoService
            yield return container.Register<string, IEchoService>(
                id => new EchoService(
                    id, 
                    container.Resolve<IEventAggregator>(),
                    container.Resolver<string, IEcho>(),
                    container.Resolve<ISubject<IEcho>>(WellknownSubject.Simple)));

            // Register ConsoleEchoPublisher as Singleton
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IEchoPublisher>(
                () => new ConsoleEchoPublisher(container.Resolve<IConsole>()));

            // Register ConsoleEchoRequestSource as Singleton
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IEchoRequestSource>(
                () => new ConsoleEchoRequestSource(container.Resolver<string, IEchoRequest>()));
        }
    }
}
