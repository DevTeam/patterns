namespace ConsoleTimer
{
    using System;
    using System.Collections.Generic;

    using DevTeam.Patterns.IoC;
    using DevTeam.Platform.System;
    
    class ConsoleTimerViaRegisterContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new SystemContainerConfiguration();
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            // Register Timer
            yield return container.Register<TimeSpan, ITimer>(
                period => new Timer(period));

            // Register Time Publisher
            yield return container.Register<ITimePublisher>(
                () => new TimePublisher(
                    container.Resolve<IConsole>(),
                    container.Resolver<TimeSpan, ITimer>()));
        }
    }
}