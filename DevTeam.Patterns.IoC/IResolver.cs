namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

	public interface IResolver
	{
	    IEnumerable<IRegistration> Registrations { get; }

	    object Resolve(Type stateType, Type instanceType, object state, string name = "");
	}
}