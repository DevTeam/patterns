namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IBinder: IContext
    {
        IRegistration Bind(IRegistry registry, Type stateType, Type contractType, Type implementationType, IFactory factory, object key = null);
    }
}
