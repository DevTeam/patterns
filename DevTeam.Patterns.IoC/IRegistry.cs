namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistry
	{        
        IDisposable Register(Type stateType, Type instanceType, Func<Type, object, object> factoryMethod, string name = "");
    }
}