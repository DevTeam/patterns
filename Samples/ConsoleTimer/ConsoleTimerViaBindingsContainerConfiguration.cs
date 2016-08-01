namespace ConsoleTimer
{
    using System;
    using System.Collections.Generic;

    using DevTeam.Patterns.IoC;
    using DevTeam.Platform.System;

    /// <inheritdoc/>
    internal class ConsoleTimerViaBindingsContainerConfiguration : IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new SystemContainerConfiguration();
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            // Register to Timer
            yield return container.Register<Timer>().As<TimeSpan, ITimer>();

            // Register to Time Publisher
            yield return container.Register<TimePublisher>().As<ITimePublisher>();
        }
    }
}
