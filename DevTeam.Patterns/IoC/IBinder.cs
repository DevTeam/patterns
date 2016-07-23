namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IBinder: IContext
    {
        IRegistration Bind(IContainer container, Type stateType, Type instanceType, Type implementationType, object key = null);
    }
}
