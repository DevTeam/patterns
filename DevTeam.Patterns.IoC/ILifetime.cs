namespace DevTeam.Patterns.IoC
{
    using System;

    public interface ILifetime: IContext
    {
        object Create(IContainer container, IKey key, Func<Type, object, object> factory, Type instanceType, object state);

        void Release(IContainer container, IKey key);
    }
}
