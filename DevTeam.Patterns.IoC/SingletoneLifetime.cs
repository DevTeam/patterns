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

        public object Create(IContainer container, IKey registryKey, Func<Type, object, object> factory, Type instanceType, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var key = new Key(registryKey, instanceType, state);
            Lazy<object> currentFactory;
            if (!_factories.TryGetValue(key, out currentFactory))
            {
                currentFactory = new Lazy<object>(() => _baseLifetime.Create(container, registryKey, factory, instanceType, state));
                _factories.Add(key, currentFactory);
            }

            return currentFactory.Value;
        }

        public void Release(IContainer container, IKey registryKey)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));

            _baseLifetime.Release(container, registryKey);
        }
        
        private class Key : IEquatable<Key>
        {
	        private readonly IKey _registryKey;
            private readonly Type _instanceType;
            private readonly object _state;

	        public Key(IKey registryKey, Type instanceType, object state)
	        {
		        if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
	            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));

	            _registryKey = registryKey;
	            _instanceType = instanceType;
	            _state = state;
	        }

            public bool Equals(Key other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(_registryKey, other._registryKey) && _instanceType == other._instanceType && Equals(_state, other._state);
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
                    var hashCode = _registryKey?.GetHashCode() ?? 0;
                    hashCode = (hashCode * 397) ^ (_instanceType?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (_state?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }
        }
    }
}