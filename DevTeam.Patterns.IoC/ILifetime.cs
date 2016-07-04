namespace DevTeam.Patterns.IoC
{
    using System;

    public interface ILifetime: IContext
    {
        object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory);

        void Release(IReleasingContext ctx);
    }
}
