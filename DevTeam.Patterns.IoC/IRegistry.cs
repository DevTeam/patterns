namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistry
	{        
        IRegistry Register(Type stateType, Type instanceType, Func<object, object> factory, string name = "");
    }
}