namespace DevTeam.TestTool.dotNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Engine.Contracts;

    public class Reflection: IReflection
    {
        public Assembly LoadAssembly(string assemblyFileName)
        {
            if (assemblyFileName == null) throw new ArgumentNullException(nameof(assemblyFileName));

            return Assembly.LoadFrom(assemblyFileName);
        }
        
        public Type LoadType(Assembly assembly, string typeName)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            return assembly.GetType(typeName);
        }

        public IEnumerable<Type> GetTypes(Assembly assembly)
        {
            return assembly.GetTypes();
        }

        public MethodInfo LoadMethod(Type type, string methodName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            return type.GetMethod(methodName);
        }

        public IEnumerable<T> GetCustomAttribute<T>(Type type)
            where T : Attribute
        {
            return type.CustomAttributes.OfType<T>();
        }

        public IEnumerable<T> GetCustomAttribute<T>(MethodInfo method)
            where T : Attribute
        {
            return method.GetCustomAttributes<T>().OfType<T>();
        }

        public IEnumerable<MethodInfo> GetMethods(Type type)
        {
            return type.GetMethods();
        }

        public object CreateInstance(Type testFixtureType)
        {
            if (testFixtureType == null) throw new ArgumentNullException(nameof(testFixtureType));

            return Activator.CreateInstance(testFixtureType);
        }
    }
}
