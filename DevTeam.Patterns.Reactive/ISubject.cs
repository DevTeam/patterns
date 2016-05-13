namespace DevTeam.Patterns.Reactive
{
    public interface ISubject<T>: IObserver<T>, IObservable<T>
    {
    }
}
