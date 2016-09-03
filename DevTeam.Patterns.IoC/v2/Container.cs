namespace DevTeam.Patterns.IoC.v2
{
    using System;
    using System.Collections.Generic;

    public class Container: IContainer
    {
        public object Key { get; }

        public IEnumerable<IRegistration> Registrations { get; }

        public IRegistration Register(IRegistrationKey key, IFactory factory, IEquatable<IExtension> extensions)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IResolvingKey key)
        {
            throw new NotImplementedException();
        }
    }
}
