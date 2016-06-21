namespace DevTeam.Patterns.Reactive
{
    using System;

    public class Event<TSource>
    {
        private Event()
        {            
        }

        public static Event<TSource> CreateOnNext(TSource value)
        {
            return new Event<TSource> {EventType = Type.OnNext, Value = value};
        }

        public static Event<TSource> CreateOnError(Exception error)
        {
            return new Event<TSource> { EventType = Type.OnError, Error = error };
        }

        public static Event<TSource> CreateOnComplete()
        {
            return new Event<TSource> { EventType = Type.OnComplete };
        }

        public Type EventType { get; private set; }

        public TSource Value { get; private set; }

        public Exception Error { get; private set; }

        public enum Type
        {
            OnNext,
            OnError,
            OnComplete
        }
    }
}
