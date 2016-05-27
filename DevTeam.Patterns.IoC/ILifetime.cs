namespace DevTeam.Patterns.IoC
{
    using System;

    public interface ILifetime
    {
        object Create(Func<object, object> factory, object state);
    }
}
