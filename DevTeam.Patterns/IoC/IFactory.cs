namespace DevTeam.Patterns.IoC
{
    using System.Reflection;

    public interface IFactory : IContext
    {
        object Create(ConstructorInfo constructor, params object[] parameters);
    }
}