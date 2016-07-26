namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistry
	{
        IRegistration Register(Type stateType, Type contractType, Func<IResolvingContext, object> factoryMethod, object key = null);
    }
}