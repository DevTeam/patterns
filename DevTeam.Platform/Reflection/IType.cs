using System;
using System.Collections.Generic;

namespace DevTeam.Platform.Reflection
{
    public interface IType
    {
        string FullName { get; }

        IEnumerable<IMethodInfo> Methods { get; }

        IEnumerable<IConstructorInfo> Constructors { get; }

        IEnumerable<T> GetCustomAttributes<T>() where T : Attribute;

        object CreateInstance();
    }
}