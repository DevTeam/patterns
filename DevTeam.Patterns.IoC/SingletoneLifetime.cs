namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
  
    internal class SingletoneLifetime : ILifetime
    {
        private readonly ILifetime _baseLifetime;
        private readonly Dictionary<Key, Lazy<object>> _factories = new Dictionary<Key, Lazy<object>>();

        public SingletoneLifetime(ILifetime baseLifetime)
        {
            if (baseLifetime == null) throw new ArgumentNullException(nameof(baseLifetime));

            _baseLifetime = baseLifetime;
        }

        public object Create(IContainer container, IRegistryKey registryKey, Func<object, object> factory, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var key = new Key(factory, state);
            Lazy<object> currentFactory;
            if (!_factories.TryGetValue(key, out currentFactory))
            {
                currentFactory = new Lazy<object>(() => _baseLifetime.Create(container, registryKey, factory, state));
                _factories.Add(key, currentFactory);
            }

            return currentFactory.Value;
        }

        public void Release(IContainer container, IRegistryKey registryKey)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));

            _baseLifetime.Release(container, registryKey);
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