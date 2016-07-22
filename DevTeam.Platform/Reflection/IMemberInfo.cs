using System;
using System.Collections.Generic;

namespace DevTeam.Platform.Reflection
{
    public interface IMemberInfo
    {
        string Name { get; }

        IEnumerable<T> GetCustomAttributes<T>() where T : Attribute;        
    }
}
