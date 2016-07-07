namespace DevTeam.Patterns.IoC
{
    using System;

    public interface ILifetime: IContainerContext
    {
        object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory);

        void Release(IReleasingContext ctx);
    }
}
