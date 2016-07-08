namespace ConsoleTimer
{
    using System;
    using System.Collections.Generic;
    
    using DevTeam.Patterns.IoC;

    /// <inheritdoc/>
    internal class ConsoleTimerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            // Register Console as Singleton
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IConsole>(
                () => new Console());

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
