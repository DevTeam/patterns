namespace DevTeam.TestTool.Engine.Runner
{
    using System;
    using System.Reflection;

    using Contracts;

    internal class MethodInfoLoader : IMethodInfoLoader
    {
        public MethodInfo Load(Type testFixtureType, TestMethod testMethod)
        {
            return testFixtureType.GetRuntimeMethod(testMethod.Name, null);
        }
    }
}
