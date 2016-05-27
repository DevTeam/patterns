namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistry
	{        
        IDisposable Register(Type stateType, Type instanceType, Func<object, object> factoryMethod, string name = "");
    }
}