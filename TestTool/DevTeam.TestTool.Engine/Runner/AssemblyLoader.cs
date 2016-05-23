namespace DevTeam.TestTool.Engine.Runner
{
    using System.Reflection;

    using Contracts;

    internal class AssemblyLoader : IAssemblyLoader
    {
        public Assembly Load(TestAssembly testAssembly)
        {
            return Assembly.Load(new AssemblyName(testAssembly.Name));
        }
    }
}
