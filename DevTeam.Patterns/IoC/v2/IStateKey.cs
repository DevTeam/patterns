namespace DevTeam.Patterns.IoC.v2
{
    using System;

    public interface IStateKey
    {
        Type StateType { get; }
    }
}
