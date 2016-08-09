namespace DevTeam.Patterns.IoC
{
    using System;

    internal class TransientLifetime : ILifetime
    {    
        public object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            return factory(ctx);
        }

        public void Release(IReleasingContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));
        }

        public override string ToString()
        {
            return $"{nameof(TransientLifetime)}";
        }
    }
}
