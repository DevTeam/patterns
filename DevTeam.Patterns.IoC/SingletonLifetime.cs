namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
  
    internal class SingletonLifetime : ILifetime
    {
        private readonly ILifetime _baseLifetime;
        private readonly Dictionary<Key, Lazy<object>> _factories = new Dictionary<Key, Lazy<object>>();

        public SingletonLifetime(ILifetime baseLifetime)
        {
            if (baseLifetime == null) throw new ArgumentNullException(nameof(baseLifetime));

            _baseLifetime = baseLifetime;
        }

        public object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var key = new Key(ctx.RegestryKey, ctx.ResolvingInstanceType, ctx.State);
            Lazy<object> currentFactory;
            if (!_factories.TryGetValue(key, out currentFactory))
            {
                currentFactory = new Lazy<object>(() => _baseLifetime.Create(ctx, factory));
                _factories.Add(key, currentFactory);
            }

            return currentFactory.Value;
        }

        public void Release(IReleasingContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            _baseLifetime.Release(ctx);
        }

        private class Key : IEquatable<Key>
        {
	        private readonly IRegestryKey _registryRegestryKey;
            private readonly Type _instanceType;
            private readonly object _state;

	        public Key(IRegestryKey registryRegestryKey, Type instanceType, object state)
	        {
		        if (registryRegestryKey == null) throw new ArgumentNullException(nameof(registryRegestryKey));
	            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));

	            _registryRegestryKey = registryRegestryKey;
	            _instanceType = instanceType;
	            _state = state;
	        }

            public bool Equals(Key other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(_registryRegestryKey, other._registryRegestryKey) && _instanceType == other._instanceType && Equals(_state, other._state);
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
                    var hashCode = _registryRegestryKey?.GetHashCode() ?? 0;
                    hashCode = (hashCode * 397) ^ (_instanceType?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (_state?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }
        }
    }
}