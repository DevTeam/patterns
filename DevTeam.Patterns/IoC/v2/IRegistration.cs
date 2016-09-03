namespace DevTeam.Patterns.IoC.v2
{
    using System;
    using System.Collections.Generic;

    public interface IRegistration: IDisposable
    {
        IRegistrationKey Key { get; }

        IContainer RegistrationContainer { get; }

        IContainer TargetContainer { get; }

        IEnumerable<IExtension> Extensions { get; }
    }
}
