namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class ControlledLifetime: ILifetime
    {
        private readonly Dictionary<IRegistration, HashSet<IDisposable>> _contracts = new Dictionary<IRegistration, HashSet<IDisposable>>();

        public object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var contract = factory(ctx);
            var disposable = contract as IDisposable;
            if (disposable == null)
            {
                return contract;
            }

            HashSet<IDisposable> contracts;
            if (!_contracts.TryGetValue(ctx.Registration, out contracts))
            {
                contracts = new HashSet<IDisposable>();
                _contracts.Add(ctx.Registration, contracts);
            }

            contracts.Add(disposable);
            return contract;
        }

        public void Release(IReleasingContext ctx)
        {
            HashSet<IDisposable> contracts;
            if (!_contracts.TryGetValue(ctx.Registration, out contracts))
            {
                return;
            }

            _contracts.Remove(ctx.Registration);
            foreach (var contract in contracts)
            {
                contract.Dispose();
            }
        }
    }
}
