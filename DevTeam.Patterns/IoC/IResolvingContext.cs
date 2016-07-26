namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolvingContext
    {
        Guid ResolvingId { get;  }

        Guid PerThreadResolvingId { get; }

        IRegistration Registration { get; }

        IResolver Resolver { get; }

        Type ResolvingContractType { get; }

        object State { get; }
    }
}