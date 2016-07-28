namespace DevTeam.Patterns.IoC
{
    using System.Reflection;

    public interface IFactory: IContext
    {
        object Create(ConstructorInfo constructor, object[] parameters);

        object ResolveState(IResolver resolver, ParameterInfo parameter, StateAttribute stateAttr, object state);

        object ResolveDependency(IResolver resolver, ParameterInfo parameter, DependencyAttribute dependencyAttr);
    }
}