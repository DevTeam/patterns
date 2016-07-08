namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolvingContext
    {
        IRegistration Registration { get; }

        IContainer ResolvingContainer { get; }

        Type ResolvingInstanceType { get; }

        object State { get; }
    }
}