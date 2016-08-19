namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    public interface IRegistrationDescription
    {
        Type ImplementationType { get; }

        IContainer Container { get; }

        WellknownLifetime Lifetime { get; }

        WellknownComparer Comparer { get; }

        WellknownScope Scope { get; }

        WellknownContractRange ContractRange { get; }

        IEnumerable<Type> AdditionalContracts { get; }
    }
}