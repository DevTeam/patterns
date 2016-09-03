﻿namespace DevTeam.Patterns.IoC.v2
{
    using System;
    using System.Collections.Generic;

    public interface IRegistry
    {
        IEnumerable<IRegistration> Registrations { get; }

        IRegistration Register(IRegistrationKey key, IFactory factory, IEquatable<IExtension> extensions);
    }
}