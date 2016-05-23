namespace DevTeam.TestTool.Engine.Contracts
{
    using System;
    using System.Reflection;

    public interface ITypeLoader
    {
        Type Load(Assembly testAssembly, TestFixture testFixture);
    }
}