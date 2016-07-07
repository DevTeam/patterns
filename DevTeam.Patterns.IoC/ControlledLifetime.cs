namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class ControlledLifetime: ILifetime
    {
        private readonly Dictionary<IRegistration, HashSet<IDisposable>> _instances = new Dictionary<IRegistration, HashSet<IDisposable>>();

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
            if (!_instances.TryGetValue(ctx.Registration, out instances))
            {
                instances = new HashSet<IDisposable>();
                _instances.Add(ctx.Registration, instances);
            }

            instances.Add(disposable);
            return instance;
        }

        public void Release(IReleasingContext ctx)
        {
            HashSet<IDisposable> instances;
            if (!_instances.TryGetValue(ctx.Registration, out instances))
            {
                return;
            }

            _instances.Remove(ctx.Registration);
            foreach (var instance in instances)
            {
                instance.Dispose();
            }
        }
    }
}
