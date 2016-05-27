namespace DevTeam.Patterns.IoC
{
    using System;    

    public class Transient : ILifetime
    {
    
        public object Create(IRegistryKey registryKey, Func<object, object> factory, object state)
        {
            return factory(state);
        }

        public void Release(IRegistryKey registryKey)
        {
        }
    }
}
