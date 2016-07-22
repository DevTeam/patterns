using System;
using System.Collections.Generic;

namespace DevTeam.Platform.Reflection
{
    public interface IMethodInfo
    {
        string Name { get; }

        IEnumerable<T> GetCustomAttributes<T>() where T : Attribute;

        object Invoke(object instance);
    }
}
