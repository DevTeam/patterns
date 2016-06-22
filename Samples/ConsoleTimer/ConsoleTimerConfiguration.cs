namespace ConsoleTimer
{
    using System;

    using DevTeam.Patterns.Dispose;
    using DevTeam.Patterns.IoC;

    internal class ConsoleTimerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            var disposable = new CompositeDisposable();

            // Register Console as Singletone
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IConsole>(
                () => new Console()));

            // Register Timer with state "period"
            disposable.Add(container.Register<TimeSpan, ITimer>(
                period => new Timer(period)));

            // Register Publisher and resolve 2 args:
            // Instance of IConsole
            // Instance of ITimer with state "period"
            disposable.Add(container.Register<ITimePublisher>(
                () => new TimePublisher(
                    container.Resolve<IConsole>(), 
                    container.Resolve<TimeSpan, ITimer>(TimeSpan.FromSeconds(1)))));

            return disposable;
        }
    }
}
