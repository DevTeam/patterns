namespace DevTeam.Patterns.Reactive
{
    using System;

    public interface ISubject<T>: IObserver<T>, IObservable<T>
    {
    }
}
