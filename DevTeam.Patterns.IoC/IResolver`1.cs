namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolver<out T>
    {
        T Resolve(IComparable name = null);
    }
}
