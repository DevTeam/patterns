namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class ControlledLifetime: ILifetime
    {
        private readonly Dictionary<IKey, HashSet<IDisposable>> _instances = new Dictionary<IKey, HashSet<IDisposable>>();

        public object Create(IContainer container, IKey key, Func<object, object> factory, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var instance = factory(state);
            var disposable = instance as IDisposable;
            if (disposable == null)
            {
                return instance;
            }

            HashSet<IDisposable> instances;
            if (!_instances.TryGetValue(key, out instances))
            {
                instances = new HashSet<IDisposable>();
                _instances.Add(key, instances);
            }

            instances.Add(disposable);
            return instance;
        }

        public void Release(IContainer container, IKey key)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (key == null) throw new ArgumentNullException(nameof(key));

            HashSet<IDisposable> instances;
            if (!_instances.TryGetValue(key, out instances))
            {
                return;
            }

            _instances.Remove(key);
            foreach (var instance in instances)
            {
                instance.Dispose();
            }
        }
    }
}
