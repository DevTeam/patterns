namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IBinder: IContext
    {
        IDisposable Bind(IContainer container, Type stateType, Type instanceType, Type implementationType, object key = null);
    }
}
