namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Reflection;

    using Contracts;

    internal class AssemblyLoader : IAssemblyLoader
    {
        public Assembly Load(string assemblyName)
        {
            if (assemblyName == null) throw new ArgumentNullException(nameof(assemblyName));

            return Assembly.Load(new AssemblyName(assemblyName));
        }
    }
}
