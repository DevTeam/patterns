namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    public interface IRegistration : IEquatable<IRegistration>, IDisposable
    {
        Type StateType { get; }

        Type ContractType { get; }

        object Key { get; }

        IEnumerable<IRegistration> GetResolveVariants();
    }
}
