namespace DevTeam.Patterns.IoC
{
    using System;

    public interface ILifetime: IContext
    {
        object Create(IContainer container, IKey key, Func<object, object> factory, object state);

        void Release(IContainer container, IKey key);
    }
}
