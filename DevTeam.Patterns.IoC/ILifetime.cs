namespace DevTeam.Patterns.IoC
{
    using System;

    public interface ILifetime
    {
        object Create(IContainer container, IRegistryKey registryKey, Func<object, object> factory, object state);

        void Release(IContainer container, IRegistryKey registryKey);
    }
}
