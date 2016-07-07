namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolvingContext
    {
        IContainer Container { get; }

        IRegistration Registration { get; }

        Type ResolvingInstanceType { get; }

        object State { get; }
    }
}