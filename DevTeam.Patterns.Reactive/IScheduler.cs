namespace DevTeam.Patterns.Reactive
{
    using System;

    public interface IScheduler
    {        
        IDisposable Schedule(Action action);
    }
}
