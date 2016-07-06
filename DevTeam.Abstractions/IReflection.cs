namespace DevTeam.Abstractions
{
    public interface IReflection
    {
        IAssembly LoadAssembly(string assemblyFileName);        
    }
}