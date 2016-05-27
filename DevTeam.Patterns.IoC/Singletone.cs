namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
  
    public class Singletone : ILifetime
    {
        private readonly Dictionary<Key, Lazy<object>> _factories = new Dictionary<Key, Lazy<object>>();
        private readonly Dictionary<IRegistryKey, HashSet<IDisposable>> _instances = new Dictionary<IRegistryKey, HashSet<IDisposable>>();
      
        public object Create(IRegistryKey registryKey, Func<object, object> factory, object state)
        {
            var key = new Key(factory, state);
            Lazy<object> currentFactory;
            if (!_factories.TryGetValue(key, out currentFactory))
            {
                currentFactory = new Lazy<object>(() => CreateInstance(registryKey, factory, state));
                _factories.Add(key, currentFactory);
            }

            return currentFactory.Value;
        }

        public void Release(IRegistryKey registryKey)
        {
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

        private object CreateInstance(IRegistryKey registryKey, Func<object, object> factory, object state)
        {
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

        private class Key
        {
            private readonly object _factory;
            private readonly object _state;

            public Key(object factory, object state)
            {
                if (factory == null) throw new ArgumentNullException(nameof(factory));

                _factory = factory;
                _state = state;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Key)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((_factory?.GetHashCode() ?? 0) * 397) ^ (_state?.GetHashCode() ?? 0);
                }
            }

            private bool Equals(Key other)
            {
                return _factory == other._factory && Equals(_state, other._state);
            }
        }
    }
}