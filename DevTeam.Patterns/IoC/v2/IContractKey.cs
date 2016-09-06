namespace DevTeam.Patterns.IoC.v2
{
    using System;

    public interface IContractKey: IKey
    {
        Type ContractType { get; }
    }
}
