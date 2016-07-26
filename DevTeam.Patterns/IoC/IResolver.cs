namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

	public interface IResolver
	{
	    IEnumerable<IRegistration> Registrations { get; }

	    object Resolve(IResolver resolver, Type stateType, Type contractType, object state, object key = null);
	}
}