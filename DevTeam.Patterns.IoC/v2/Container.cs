namespace DevTeam.Patterns.IoC.v2
{
    using System;
    using System.Collections.Generic;

    public class Container: IContainer
    {
        public Container(object key = null)
        {
            Key = key;
        }

        public object Key { get; }

        public IEnumerable<IRegistration> Registrations { get; }

        public IRegistration Register(IRegistrationKey key, IInstaceFactory instaceFactory, IEquatable<IExtension> extensions)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IResolvingKey key)
        {
            throw new NotImplementedException();
        }
    }
}
