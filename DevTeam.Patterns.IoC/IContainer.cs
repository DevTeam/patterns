namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IContainer : IResolver, IRegistry, IDisposable
    {
        string Name { get; }
    }
}