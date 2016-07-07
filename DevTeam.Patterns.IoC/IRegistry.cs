namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistry
	{
        IRegistration Register(Type stateType, Type instanceType, Func<IResolvingContext, object> factoryMethod, IComparable key = null);
    }
}