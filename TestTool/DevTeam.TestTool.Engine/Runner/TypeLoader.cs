namespace DevTeam.TestTool.Engine.Runner
{
    using System;
    using System.Reflection;

    using Contracts;

    internal class TypeLoader : ITypeLoader
    {
        public Type Load(Assembly testAssembly, TestFixture testFixture)
        {
            return testAssembly.GetType(testFixture.Name);            
        }
    }
}
