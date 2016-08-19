namespace Echo
{
    using System.Collections.Generic;

    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.IoC;
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
            yield return container.Register<EchoRequest>().As<string, IEchoRequest>();

            // Register Echo
            yield return container.Register<Echo>().As<string, IEcho>();

            // Register Echo Service
            yield return container.Register<EchoService>().As<string, IEchoService>();

            // Register Console Echo Publisher as Singleton
            yield return Registries.As<IEchoPublisher>(container.Register<ConsoleEchoPublisher>(WellknownLifetime.Singleton));

            // Register Console Echo Request bbSource as Singleton
            yield return Registries.As<IEchoRequestSource>(container.Register<ConsoleEchoRequestSource>(WellknownLifetime.Singleton));
        }
    }
}
