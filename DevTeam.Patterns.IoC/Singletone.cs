namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
  
    public class Singletone : IFactory
    {
        private readonly Dictionary<object, Lazy<object>> _factories;
      
        public Singletone()
        {
            _factories = new Dictionary<object, Lazy<object>>();
        }

        public object Create(Func<object, object> factory, object state)
        {
            Lazy<object> currentFactory;
            if (!_factories.TryGetValue(state, out currentFactory))
            {
                currentFactory = new Lazy<object>(() => factory(state));
                _factories.Add(state, currentFactory);
            }

            return currentFactory.Value;
        }
    }
}