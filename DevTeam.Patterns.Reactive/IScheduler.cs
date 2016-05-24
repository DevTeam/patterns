namespace DevTeam.Patterns.Reactive
{
    using System;

    public interface IScheduler
    {        
        void Schedule(Action action);
    }
}
