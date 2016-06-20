namespace DevTeam.TestTool.Engine.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public interface IReflection
    {
        object CreateInstance(Type type);        

        Assembly LoadAssembly(string assemblyFileName);

        MethodInfo LoadMethod(Type type, string methodName);

        Type LoadType(Assembly assembly, string typeName);

        IEnumerable<Type> GetTypes(Assembly assembly);

        IEnumerable<T> GetCustomAttribute<T>(Type type) where T : Attribute;

        IEnumerable<T> GetCustomAttribute<T>(MethodInfo method) where T : Attribute;

        IEnumerable<MethodInfo> GetMethods(Type type);        
    }
}