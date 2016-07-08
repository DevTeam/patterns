namespace DevTeam.Patterns.Reactive
{
    using System;

    /// <summary>
    /// Represents an object that schedules units of work.
    /// </summary>
    public interface IScheduler
    {
        /// <summary>
        /// Schedules an action to be executed.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        /// <returns>Еhe disposable token used to cancel the scheduled action.</returns>
        IDisposable Schedule(Action action);
    }
}
