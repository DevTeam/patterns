namespace DevTeam.Platform.Reflection
{
    public interface IMethodInfo: IMemberInfo
    {
        object Invoke(object instance, params object[] parameters);
    }
}
