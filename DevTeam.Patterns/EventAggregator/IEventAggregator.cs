namespace DevTeam.Patterns.EventAggregator
{
    using System;

    /// <summary>
    /// The IEventAggregator is primarily a container for events that allow decoupling of publishers and subscribers so they can evolve independently.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Registers a provider of events.
        /// </summary>
        /// <typeparam name="T">The type of events.</typeparam>
        /// <param name="provider">The provider of events.</param>
        /// <returns>The registration token.</returns>
        IDisposable RegisterProvider<T>(IObservable<T> provider);

        /// <summary>
        /// Registers a consumer of events.
        /// </summary>
        /// <typeparam name="T">The type of events.</typeparam>
        /// <param name="consumer">The consumer of events.</param>
        /// <returns>The registration token.</returns>
        IDisposable RegisterConsumer<T>(IObserver<T> consumer);
    }
}