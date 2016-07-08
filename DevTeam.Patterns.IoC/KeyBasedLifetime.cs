namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
  
    internal abstract class KeyBasedLifetime: ILifetime
    {
        private readonly ILifetime _baseLifetime;
        private readonly Dictionary<object, Lazy<object>> _factories = new Dictionary<object, Lazy<object>>();

        protected KeyBasedLifetime(ILifetime baseLifetime)
        {
            if (baseLifetime == null) throw new ArgumentNullException(nameof(baseLifetime));
            
            _baseLifetime = baseLifetime;            
        }

        public object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var key = CreateKey(ctx);
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

        protected abstract object CreateKey(IResolvingContext ctx);
    }
}