namespace IoC.Contracts
{
    using System;

    public interface IContractKey
    {
        Type ContractType { get; }
    }
}
