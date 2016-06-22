namespace ConsoleTimer
{
    using System;

    internal interface ITimer: IObservable<DateTimeOffset>
    {
    }
}