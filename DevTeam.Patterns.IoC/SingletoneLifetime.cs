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

        public object Create(IContainer container, IKey registryKey, Func<object, object> factory, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var key = new Key(registryKey, state);
            Lazy<object> currentFactory;
            if (!_factories.TryGetValue(key, out currentFactory))
            {
                currentFactory = new Lazy<object>(() => _baseLifetime.Create(container, registryKey, factory, state));
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
        
        private class Key
        {
	        private readonly IKey _registryKey;
	        private readonly object _state;

	        public Key(IKey registryKey, object state)
	        {
		        if (registryKey == null) throw new ArgumentNullException(nameof(registryKey));

		        _registryKey = registryKey;
		        _state = state;
	        }

	        public override bool Equals(object obj)
	        {
		        if (ReferenceEquals(null, obj)) return false;
		        if (ReferenceEquals(this, obj)) return true;
		        if (obj.GetType() != GetType()) return false;
		        return Equals((Key) obj);
	        }

	        public override int GetHashCode()
	        {
		        unchecked
		        {
			        return ((_registryKey?.GetHashCode() ?? 0)*397) ^ (_state?.GetHashCode() ?? 0);
		        }
	        }

	        private bool Equals(Key other)
	        {
		        return Equals(_registryKey, other._registryKey) && Equals(_state, other._state);
	        }
        }
    }
}