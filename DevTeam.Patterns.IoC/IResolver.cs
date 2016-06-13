namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

	public interface IResolver
	{        
        object Resolve(Type stateType, Type instanceType, object state, string name = "");

		IEnumerable<Tuple<IRegistryKey, object>> Resolve(Func<IRegistryKey, bool> filter, Func<IRegistryKey, object> stateSelector);
	}
}