namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class ControlledLifetime: ILifetime
    {
        private readonly Dictionary<IRegestryKey, HashSet<IDisposable>> _instances = new Dictionary<IRegestryKey, HashSet<IDisposable>>();

        public object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var instance = factory(ctx);
            var disposable = instance as IDisposable;
            if (disposable == null)
            {
                return instance;
            }

            HashSet<IDisposable> instances;
            if (!_instances.TryGetValue(ctx.RegestryKey, out instances))
            {
                instances = new HashSet<IDisposable>();
                _instances.Add(ctx.RegestryKey, instances);
            }

            instances.Add(disposable);
            return instance;
        }

        public void Release(IReleasingContext ctx)
        {
            HashSet<IDisposable> instances;
            if (!_instances.TryGetValue(ctx.RegestryKey, out instances))
            {
                return;
            }

            _instances.Remove(ctx.RegestryKey);
            foreach (var instance in instances)
            {
                instance.Dispose();
            }
        }
    }
}
