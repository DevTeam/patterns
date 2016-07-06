namespace DevTeam.Abstractions.Reflection
{
    public interface IReflection
    {
        IAssembly LoadAssembly(string assemblyFileName);        
    }
}