namespace ConsoleTimer
{
    using System;
    using System.Collections.Generic;

    using DevTeam.Patterns.IoC;

    internal class ConsoleTimerConfiguration: IConfiguration
    {
        public static readonly IConfiguration Shared = new ConsoleTimerConfiguration();

        private ConsoleTimerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            // Register Console as Singletone
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConsole>(
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
