namespace IoC.Contracts
{
    using System.Collections.Generic;

    public interface IKey
    {
        IEnumerable<IContractKey> Contracts { get; }

        IEnumerable<IStateKey> States { get; }

        object Key { get; }
    }
}