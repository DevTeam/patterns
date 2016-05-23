namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Reflection;

    using Contracts;

    internal class TypeLoader : ITypeLoader
    {
        public Type Load(Assembly assembly, string typeName)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            return assembly.GetType(typeName);
        }
    }
}
