namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistrationFactory
    {
        IRegistration Create(IRegistrationDescription description, Type stateType, Type contractType, object key = null);
    }
}
