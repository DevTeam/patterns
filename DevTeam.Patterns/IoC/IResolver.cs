namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IResolver
    {
        object Resolve(Type stateType, Type contractType, object state, object key = null);

        object Resolve(IContainer resolverContainer, IRegistration registration, object state);
    }
}