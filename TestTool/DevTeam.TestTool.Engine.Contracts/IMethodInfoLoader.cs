namespace DevTeam.TestTool.Engine.Contracts
{
    using System;
    using System.Reflection;

    public interface IMethodInfoLoader
    {
        MethodInfo Load(Type type, string methodName);
    }
}