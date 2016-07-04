namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegestryKey: IEquatable<IRegestryKey>
    {
        Type StateType { get; }

        Type InstanceType { get; }

        string Name { get; }
    }
}
