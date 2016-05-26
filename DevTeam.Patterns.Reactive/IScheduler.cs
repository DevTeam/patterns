namespace DevTeam.Patterns.Reactive
{
    using System;

    public interface IScheduler: IDisposable
    {        
        IDisposable Schedule(Action action);
    }
}
