namespace DevTeam.Patterns.Reactive
{
    using System;

    /// <summary>
    /// Represents an object that is both an observable sequence as well as an observer.
    /// </summary>
    /// <typeparam name="T">The type of source.</typeparam>
    public interface ISubject<T>: IObserver<T>, IObservable<T>
    {
    }
}
