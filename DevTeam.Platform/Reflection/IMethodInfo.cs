namespace DevTeam.Platform.Reflection
{
    using System;
    using System.Collections.Generic;

    public interface IMethodInfo
    {
        string Name { get; }

        IEnumerable<T> GetCustomAttributes<T>() where T : Attribute;

        object Invoke(object instance);
    }
}
