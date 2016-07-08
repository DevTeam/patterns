namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
  
    internal class SingletonLifetime : ILifetime
    {
        private readonly ILifetime _baseLifetime;
        private readonly Dictionary<object, Lazy<object>> _factories = new Dictionary<object, Lazy<object>>();

        public SingletonLifetime(ILifetime baseLifetime)
        {
            if (baseLifetime == null) throw new ArgumentNullException(nameof(baseLifetime));

            _baseLifetime = baseLifetime;
        }

        public object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var key = new Key(ctx);
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

        private class Key
        {
            private readonly IResolvingContext _ctx;
            
	        public Key(IResolvingContext ctx)
	        {
	            _ctx = ctx;	            
	        }
            
            public bool Equals(Key other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(_ctx.Registration, other._ctx.Registration) && _ctx.ResolvingInstanceType == other._ctx.ResolvingInstanceType && Equals(_ctx.State, other._ctx.State);
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
                    var hashCode = _ctx.Registration?.GetHashCode() ?? 0;
                    hashCode = (hashCode * 397) ^ (_ctx.ResolvingInstanceType?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (_ctx.State?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }
        }
    }
}