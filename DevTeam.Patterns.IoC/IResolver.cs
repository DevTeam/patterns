namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolver
	{        
        object Resolve(Type stateType, Type instanceType, object state, string name = "");
    }
}