namespace IoC.Contracts
{
    using System;

    public interface IStateKey
    {
        Type StateType { get; }
    }
}
