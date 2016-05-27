namespace DevTeam.Patterns.IoC
{
    using System;

    public interface ILifetime
    {
        object Create(IRegistryKey registryKey, Func<object, object> factory, object state);

        void Release(IRegistryKey registryKey);
    }
}
