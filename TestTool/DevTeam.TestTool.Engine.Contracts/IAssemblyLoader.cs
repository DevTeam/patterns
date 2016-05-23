namespace DevTeam.TestTool.Engine.Contracts
{
    using System.Reflection;

    public interface IAssemblyLoader
    {
        Assembly Load(string assemblyName);
    }
}