namespace IoC.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface IContainer: IRegistry, IResolver, IDisposable
    {
        object Key { get; }

        IEnumerable<IRegistration> Registrations { get; }
    }
}