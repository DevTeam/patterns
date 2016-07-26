namespace ConsoleTimer
{
    using System;

    public interface ITimer: IObservable<DateTimeOffset>
    {
    }
}