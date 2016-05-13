namespace DevTeam.Patterns.Reactive
{
    using System;

    public interface IObserver<in T>
    {
        void OnNext(T value);

        void OnError(Exception error);

        void OnCompleted();
    }
}
