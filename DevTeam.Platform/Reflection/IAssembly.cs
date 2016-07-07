﻿namespace DevTeam.Platform.Reflection
{
    using System.Collections.Generic;

    public interface IAssembly
    {
        IEnumerable<IType> DefinedTypes { get; }

        IType GetType(string typeName);
    }
}