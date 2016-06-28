namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Contracts;

    public class Reflection: IReflection
    {
        public Assembly LoadAssembly(string assemblyFileName)
        {
            if (assemblyFileName == null) throw new ArgumentNullException(nameof(assemblyFileName));

            return Assembly.Load(new AssemblyName(assemblyFileName));
        }
        
        public Type LoadType(Assembly assembly, string typeName)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            return assembly.GetType(typeName);
        }

        public IEnumerable<Type> GetTypes(Assembly assembly)
        {
            return assembly.DefinedTypes.Select(i => i.AsType());
        }

        public MethodInfo LoadMethod(Type type, string methodName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            return type.GetTypeInfo().GetDeclaredMethod(methodName);
        }

        public IEnumerable<T> GetCustomAttribute<T>(Type type)
            where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttributes<T>();
        }

        public IEnumerable<T> GetCustomAttribute<T>(MethodInfo method)
            where T : Attribute
        {
            return method.GetCustomAttributes<T>();
        }

        public IEnumerable<MethodInfo> GetMethods(Type type)
        {
            return type.GetTypeInfo().DeclaredMethods;
        }

        public object CreateInstance(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return Activator.CreateInstance(type);
        }
    }
}
