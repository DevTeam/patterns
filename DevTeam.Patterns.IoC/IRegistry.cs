namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistry
	{        
        IDisposable Register(Type stateType, Type instanceType, Func<IResolvingContext, object> factoryMethod, string name = "");
    }
}