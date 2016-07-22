namespace DevTeam.Platform.Reflection
{
    public interface IReflection
    {
        IAssembly LoadAssembly(string assemblyFileName);

        IType GetType(string typeName, bool throwOnError);
    }
}