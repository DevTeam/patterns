namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

	public interface IResolver
	{
	    IEnumerable<IKey> Keys { get; }

	    object Resolve(Type stateType, Type instanceType, object state, string name = "");
	}
}