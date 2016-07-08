namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolver<in TState, out T>
    {
        T Resolve(TState state, object key = null);
    }
}
