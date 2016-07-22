namespace DevTeam.Platform.Reflection
{
    public interface IConstructorInfo: IMemberInfo
    {
        object Invoke(params object[] parameters);
    }
}
