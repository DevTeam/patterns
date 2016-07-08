namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IReleasingContext
    {
        IContainer Container { get; }

        IRegistration Registration { get; }

        object Key { get; }
    }
}