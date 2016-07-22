namespace ConsoleTimer
{
    using System;
    using System.Collections.Generic;
    
    using DevTeam.Patterns.IoC;
    using DevTeam.Platform.System;

    /// <inheritdoc/>
    internal class ConsoleTimerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new SystemContainerConfiguration();
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            // Register Timer with state "period"
            yield return container.Register<TimeSpan, ITimer>(
                period => new Timer(period));

            // Register Publisher and resolve 2 args:
            // Instance of IConsole
            // Instance of ITimer with state "period"
            yield return container.Register<ITimePublisher>(
                () => new TimePublisher(
                    container.Resolve<IConsole>(), 
                    container.Resolve<TimeSpan, ITimer>(TimeSpan.FromSeconds(1))));
        }
    }
}
