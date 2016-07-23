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
            // Register Echo Request
            yield return container.Bind<string, IEchoRequest, EchoRequest>();

            // Register Echo
            yield return container.Bind<string, IEcho, Echo>();

            // Register Echo Service
            yield return container.Bind<string, IEchoService, EchoService>();

            // Register Console Echo Publisher as Singleton
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IEchoPublisher, ConsoleEchoPublisher>();

            // Register Console Echo Request bbSource as Singleton
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Bind<IEchoRequestSource, ConsoleEchoRequestSource>();
        }
    }
}
