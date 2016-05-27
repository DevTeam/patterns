namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IFactory
    {
        object Create(Func<object, object> factory, object state);
    }
}
