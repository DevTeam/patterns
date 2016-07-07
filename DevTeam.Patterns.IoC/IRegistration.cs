﻿namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistration: IEquatable<IRegistration>, IDisposable
    {
        Type StateType { get; }

        Type InstanceType { get; }

        IComparable Key { get; }
    }
}