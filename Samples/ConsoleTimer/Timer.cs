namespace ConsoleTimer
{
    using System;

    using DevTeam.Patterns.IoC;

    internal class Timer: ITimer
    {
        private readonly TimeSpan _period;

        public Timer([State] TimeSpan period)
        {
            _period = period;
        }

        public IDisposable Subscribe(IObserver<DateTimeOffset> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            // Create a timer
            var realTimer = new System.Threading.Timer(
                state => {
                    var curObserver = (IObserver<DateTimeOffset>)state;
                    curObserver.OnNext(DateTimeOffset.Now);
                },
                observer,
                TimeSpan.Zero,
                _period);

            // Return a timer as subscription token
            return realTimer;
        }
    }
}
