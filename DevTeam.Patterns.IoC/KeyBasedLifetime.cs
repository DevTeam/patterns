namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal abstract class KeyBasedLifetime: ILifetime
    {
        private readonly ILifetime _baseLifetime;
        private readonly Dictionary<IRegistration, IDictionary<object, Lazy<object>>> _factories = new Dictionary<IRegistration, IDictionary<object, Lazy<object>>>();
        
        protected KeyBasedLifetime(ILifetime baseLifetime)
        {
            if (baseLifetime == null) throw new ArgumentNullException(nameof(baseLifetime));
            
            _baseLifetime = baseLifetime;
        }

        public object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            IDictionary<object, Lazy<object>> registrationFactories;
            if (!_factories.TryGetValue(ctx.Registration, out registrationFactories))
            {
                registrationFactories = new Dictionary<object, Lazy<object>>();
                _factories.Add(ctx.Registration, registrationFactories);
            }

            var key = CreateKey(ctx);
            Lazy<object> currentFactory;
            if (!registrationFactories.TryGetValue(key, out currentFactory))
            {
                currentFactory = new Lazy<object>(() => _baseLifetime.Create(ctx, factory));
                registrationFactories.Add(key, currentFactory);
            }

            return currentFactory.Value;
        }

        public void Release(IReleasingContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            if (_factories.Remove(ctx.Registration))
            {
                _baseLifetime.Release(ctx);
            }
        }

        protected abstract object CreateKey(IResolvingContext ctx);

        public override string ToString()
        {
            return $"Contracts: {_factories.Count}, Instances: {_factories.Values.SelectMany(i => i).Count()}";
        }
    }
}