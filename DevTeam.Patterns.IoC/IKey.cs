namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IKey: IEquatable<IKey>
    {
        Type StateType { get; }

        Type InstanceType { get; }

        Type ResolvingInstanceType { get; }        

        string Name { get; }
    }
}
