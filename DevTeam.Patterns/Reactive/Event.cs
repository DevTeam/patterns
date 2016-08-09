namespace DevTeam.Patterns.Reactive
{
    using System;

    /// <summary>
    /// Container of observable events.
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class Event<TSource>
    {
        private TSource _value;

        private Exception _error;

        private Event()
        {
        }

        /// <summary>
        /// Creates OnNext event.
        /// </summary>
        /// <param name="value">The value of event.</param>
        /// <returns>The OnNext event.</returns>
        public static Event<TSource> CreateOnNext(TSource value)
        {
            return new Event<TSource> { EventType = Type.OnNext, Value = value };
        }

        /// <summary>
        /// Creates Error event.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>The Error event.</returns>
        public static Event<TSource> CreateOnError(Exception error)
        {
            return new Event<TSource> { EventType = Type.OnError, Error = error };
        }

        /// <summary>
        /// Creates OnComplete event;
        /// </summary>
        /// <returns>The OnComplete event.</returns>
        public static Event<TSource> CreateOnComplete()
        {
            return new Event<TSource> { EventType = Type.OnComplete };
        }

        /// <summary>
        /// Gets the type of event.
        /// </summary>
        public Type EventType { get; private set; }

        /// <summary>
        /// Gets the value of the OnNext event.
        /// </summary>
        public TSource Value
        {
            get
            {
                if (EventType != Type.OnNext)
                {
                    throw new InvalidOperationException($"{nameof(Value)} is available only for a {nameof(Type.OnNext)} event.");
                }

                return _value;
            }

            private set
            {
                _value = value;
            }
        }

        /// <summary>
        /// Get the error of the Error event.
        /// </summary>
        public Exception Error
        {
            get
            {
                if (EventType != Type.OnError)
                {
                    throw new InvalidOperationException($"{nameof(Error)} is available only for a {nameof(Type.OnError)} event.");
                }

                return _error;
            }
            private set
            {
                _error = value;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Event<TSource>)} [Value: {Value?.ToString() ?? "null"}, Error: {Error?.ToString() ?? "null"}]";
        }

        /// <summary>
        /// Types of observable event.
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Indicates the source provides a new value.
            /// </summary>
            OnNext,

            /// <summary>
            /// Indicates the source in the error state.
            /// </summary>
            OnError,

            /// <summary>
            /// Indicates the source is completed.
            /// </summary>
            OnComplete
        }
    }
}
