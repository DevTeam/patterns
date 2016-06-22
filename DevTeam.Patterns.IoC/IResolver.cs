namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

	public interface IResolver
	{
	    IEnumerable<IRegistryKey> Keys { get; }

	    object Resolve(Type stateType, Type instanceType, object state, string name = "");
	}
}