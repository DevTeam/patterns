namespace DevTeam.Patterns.IoC
{
    using System;

    internal class ResolvingContext : IResolvingContext
    {
        public ResolvingContext(IContainer container, IRegestryKey regestryKey, Type resolvingInstanceType, object state)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (regestryKey == null) throw new ArgumentNullException(nameof(regestryKey));            
            if (resolvingInstanceType == null) throw new ArgumentNullException(nameof(resolvingInstanceType));

            Container = container;
            RegestryKey = regestryKey;            
            ResolvingInstanceType = resolvingInstanceType;
            State = state;
        }

        public IContainer Container { get; }

        public IRegestryKey RegestryKey { get; }

        public string Name { get; }

        public Type ResolvingInstanceType { get; }

        public object State { get; }
    }
}
