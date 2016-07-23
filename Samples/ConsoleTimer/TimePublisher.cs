namespace ConsoleTimer
{
    using System;

    using DevTeam.Patterns.IoC;
    using DevTeam.Patterns.Reactive;
    using DevTeam.Platform.System;

    internal class TimePublisher : ITimePublisher
    {
        private readonly IDisposable _subscription;

        public TimePublisher(
            [Dependency] IConsole console,
            [Dependency] IResolver<TimeSpan, ITimer> timer)
        {
            if (console == null) throw new ArgumentNullException(nameof(console));
            if (timer == null) throw new ArgumentNullException(nameof(timer));

            _subscription = timer.Resolve(TimeSpan.FromSeconds(1)).Subscribe(
                // Process a new item from timer
                i => console.WriteLine(i.ToString()), 
                // Process an error
                e => {},
                // Process a completion
                () => {});
        }

        public void Dispose()
        {
            // Destroy a subscription
            _subscription.Dispose();
        }
    }
}
