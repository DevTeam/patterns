namespace DevTeam.Patterns.IoC
{
    using System;

    internal class TransientLifetime : ILifetime
    {
    
        public object Create(IContainer container, IKey key, Func<object, object> factory, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            return factory(state);
        }

        public void Release(IContainer container, IKey key)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (key == null) throw new ArgumentNullException(nameof(key));
        }
    }
}
