namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IContainer : IResolver, IRegistry, IDisposable
    {
        IComparable Key { get; }
    }
}