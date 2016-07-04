namespace DevTeam.Patterns.IoC
{
    using System;

    public interface ILifetime: IConfigurationContext
    {
        object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory);

        void Release(IReleasingContext ctx);
    }
}
