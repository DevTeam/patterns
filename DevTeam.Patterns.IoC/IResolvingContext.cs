namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolvingContext
    {
        IContainer Container { get; }

        IRegestryKey RegestryKey { get; }

        Type ResolvingInstanceType { get; }

        object State { get; }
    }
}