namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class ControlledLifetime: ILifetime
    {
        private readonly Dictionary<IRegistryKey, HashSet<IDisposable>> _instances = new Dictionary<IRegistryKey, HashSet<IDisposable>>();

        public object Create(IContainer container, IRegistryKey registryKey, Func<object, object> factory, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var instance = factory(state);
            var disposable = instance as IDisposable;
            if (disposable == null)
            {
                return instance;
            }

            HashSet<IDisposable> instances;
            if (!_instances.TryGetValue(registryKey, out instances))
            {
                instances = new HashSet<IDisposable>();
                _instances.Add(registryKey, instances);
            }

            instances.Add(disposable);
            return instance;
        }

        public void Release(IContainer container, IRegistryKey registryKey)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));

            HashSet<IDisposable> instances;
            if (!_instances.TryGetValue(registryKey, out instances))
            {
                return;
            }

            _instances.Remove(registryKey);
            foreach (var instance in instances)
            {
                instance.Dispose();
            }
        }
    }
}
