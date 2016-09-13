namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistry
    {
        IRegistration Register(IContainer registerContainer, IRegistration registration, Func<IResolvingContext, object> factoryMethod);
    }
}