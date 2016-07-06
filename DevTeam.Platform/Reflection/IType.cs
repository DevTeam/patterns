namespace DevTeam.Platform.Reflection
{
    using System;
    using System.Collections.Generic;

    public interface IType
    {
        string FullName { get; }

        IEnumerable<IMethodInfo> Methods { get; }        

        IEnumerable<T> GetCustomAttributes<T>() where T : Attribute;

        object CreateInstance();
    }
}