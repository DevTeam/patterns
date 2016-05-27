namespace DevTeam.Patterns.IoC
{
    using System;

    internal class TransientLifetime : ILifetime
    {
    
        public object Create(IContainer container, IRegistryKey registryKey, Func<object, object> factory, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            return factory(state);
        }

        public void Release(IContainer container, IRegistryKey registryKey)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
        }
    }
}
