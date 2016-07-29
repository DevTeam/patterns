﻿namespace ConsoleTimer
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
            // Bind to Timer
            yield return container.Bind<TimeSpan, ITimer, Timer>();

            // Bind to Time Publisher
            yield return container.Bind<ITimePublisher, TimePublisher>();
        }
    }
}