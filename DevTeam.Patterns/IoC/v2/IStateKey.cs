namespace DevTeam.Patterns.IoC.v2
{
    using System;

    public interface IStateKey : IKey
    {
        Type StateType { get; }
    }
}