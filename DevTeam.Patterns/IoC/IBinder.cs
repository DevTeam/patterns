namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IBinder: IContext
    {
        IRegistration Bind(IRegistry registry, Type stateType, Type contractType, Type implementationType, object key = null);
    }
}
