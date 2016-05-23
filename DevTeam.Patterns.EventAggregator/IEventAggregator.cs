namespace DevTeam.Patterns.EventAggregator
{
    using System;

    public interface IEventAggregator
    {
        IDisposable RegisterProvider<T>(IObservable<T> provider);

        IDisposable RegisterConsumer<T>(IObserver<T> consumer);
    }
}