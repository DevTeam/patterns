namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IBinder : IContext
    {
        IRegistration Bind(IContainer container, Type stateType, Type contractType, Type implementationType, IFactory factory, object key = null);
    }
}
