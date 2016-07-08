namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolvingContext
    {
        Guid ResolvingId { get;  }

        Guid PerThreadResolvingId { get; }

        IRegistration Registration { get; }

        IContainer ResolvingContainer { get; }

        Type ResolvingInstanceType { get; }

        object State { get; }
    }
}