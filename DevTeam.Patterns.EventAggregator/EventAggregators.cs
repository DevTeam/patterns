namespace DevTeam.Patterns.EventAggregator
{
    using System;
    using System.Collections.Generic;

    using Reactive;

    /// <summary>
    /// EventAggregator's extensions.
    /// </summary>
    public static class EventAggregators
    {
        /// <summary>
        /// Publish items from the source to the EventAggregator.
        /// </summary>
        /// <param name="eventAggregator">Target EventAggregator.</param>
        /// <param name="source">The source of items.</param>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <returns>Target EventAggregator.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEventAggregator Publish<T>(this IEventAggregator eventAggregator, IEnumerable<T> source)
        {
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (source == null) throw new ArgumentNullException(nameof(source));

            using (eventAggregator.RegisterProvider(source.ToObservable()))
            {
            }

            return eventAggregator;
        }

        /// <summary>
        /// Publish items to the EventAggregator.
        /// </summary>
        /// <param name="eventAggregator">Target EventAggregator.</param>
        /// <param name="items">The source of items.</param>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <returns>Target EventAggregator.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEventAggregator Publish<T>(this IEventAggregator eventAggregator, params T[] items)
        {
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (items == null) throw new ArgumentNullException(nameof(items));

            return eventAggregator.Publish((IEnumerable<T>)items);
        }
    }
}
