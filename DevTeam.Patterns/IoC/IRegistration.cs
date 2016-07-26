namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistration: IEquatable<IRegistration>, IDisposable
    {
        Type StateType { get; }

        Type ContractType { get; }

        object Key { get; }
    }
}
