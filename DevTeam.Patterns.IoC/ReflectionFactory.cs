namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Reflection;

    internal class ReflectionFactory: IFactory
    {
        public object Create(ConstructorInfo constructor, object[] parameters)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            return constructor.Invoke(parameters);
        }

        public object ResolveState(IResolver resolver, ParameterInfo parameter, StateAttribute stateAttr, object state)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (stateAttr == null) throw new ArgumentNullException(nameof(stateAttr));

            return state;
        }

        public object ResolveDependency(IResolver resolver, ParameterInfo parameter, DependencyAttribute dependencyAttr)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (dependencyAttr == null) throw new ArgumentNullException(nameof(dependencyAttr));

            return resolver.Resolve(
                resolver,
                dependencyAttr.StateType ?? typeof(EmptyState),
                dependencyAttr.ContractType ?? parameter.ParameterType,
                dependencyAttr.State ?? EmptyState.Shared,
                dependencyAttr.Key);
        }
    }
}