namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolvingContext
    {
        long ResolvingId { get; }

        long PerThreadResolvingId { get; }

        IRegistration Registration { get; }

        IContainer RegisterContainer { get; }

        IContainer ResolverContainer { get; }

        Type ResolvingContractType { get; }

        object State { get; }
    }
}