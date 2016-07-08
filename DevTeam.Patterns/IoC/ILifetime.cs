namespace DevTeam.Patterns.IoC
{
    using System;

    /// <summary>
    /// Represents an abstraction for liftime manager.
    /// </summary>
    public interface ILifetime: IContext
    {
        object Create(IResolvingContext ctx, Func<IResolvingContext, object> factory);

        void Release(IReleasingContext ctx);
    }
}
