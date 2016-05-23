namespace DevTeam.TestTool.Engine.Contracts
{
    using System;
    using System.Reflection;

    public interface IMethodInfoLoader
    {
        MethodInfo Load(Type testFixtureType, TestMethod testMethod);
    }
}