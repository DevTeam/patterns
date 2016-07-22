namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class RefletionBinder: IBinder
    {
        public IDisposable Bind(IContainer container, Type stateType, Type instanceType, Type implementationType, object key = null)
        {
            ConstructorInfo resolvingConstructor = null;
            var ctors = implementationType.GetTypeInfo().DeclaredConstructors.ToList();
            if (ctors.Count == 1)
            {
                resolvingConstructor = ctors[0];
            }
            else
            {
                var resolvingConstructors = (
                    from ctor in ctors
                    let resolverAttribute = ctor.GetCustomAttribute<ResolverAttribute>()
                    where resolverAttribute != null
                    select ctor).ToList();

                if (resolvingConstructors.Count == 0)
                {
                    throw new InvalidOperationException("Resolving constructor was not found.");
                }

                if (resolvingConstructors.Count > 1)
                {
                    throw new InvalidOperationException("Shuld be only one resolving constructord.");
                }

                resolvingConstructor = resolvingConstructors[0];
            }

            var ctorParameters = resolvingConstructor.GetParameters().Select(parameter => new CtorParameter(parameter)).ToList();
            var invalidParameters = ctorParameters.Where(i => !i.ResolverParameter).ToList();
            if (invalidParameters.Any())
            {
                throw new InvalidOperationException($"Parameters {CreateParametersList(invalidParameters)} of resolving constructor has no attributes of types \"{typeof(DependencyAttribute).Name}\" or \"{typeof(StateAttribute).Name}\"");
            }

            if (stateType != typeof(EmptyState))
            {
                var stateParamaters = (
                    from parameter in ctorParameters
                    where parameter.State != null
                    select 
                    
                    
                    
                    parameter).ToList();

                if (stateParamaters.Count != 1)
                {
                    throw new InvalidOperationException($"Constructor should have only one state parameter, but has more: {CreateParametersList(stateParamaters)}");
                }

                var stateParamater = stateParamaters.Single();
                if (stateParamater.Parameter.ParameterType != stateType)
                {
                    throw new InvalidOperationException($"State parameter \"{stateParamater.Parameter.Name}\" has invalid type \"{stateParamater.Parameter.ParameterType.Name}\", but should have \"{stateType}\".");
                }
            }

            return container.Register(stateType, instanceType,
                ctx =>
                {
                    var parameters = ctorParameters.Select(i => ResolveParameter(container, ctx.State, i)).ToArray();
                    return resolvingConstructor.Invoke(parameters);
                },
                key);
        }

        private static object ResolveParameter(IContainer container, object state, CtorParameter parameter)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            if (parameter.State != null)
            {
                return state;
            }

            if (parameter.Dependency != null)
            {
                var dependency = parameter.Dependency;
                return container.Resolve(
                    dependency.StateType,
                    dependency.InstanceType ?? parameter.Parameter.ParameterType,
                    null,
                    dependency.Key);
            }

            throw new InvalidOperationException("Fail to resolve");
        }

        private static string CreateParametersList(IEnumerable<CtorParameter> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            return string.Join(", ", parameters.Select(i => $"\"{i}\""));
        }

        private class CtorParameter
        {
            public CtorParameter(ParameterInfo parameter)
            {
                if (parameter == null) throw new ArgumentNullException(nameof(parameter));

                Parameter = parameter;
                Dependency = parameter.GetCustomAttribute<DependencyAttribute>();
                State = parameter.GetCustomAttribute<StateAttribute>();
            }

            public ParameterInfo Parameter { get; }

            public DependencyAttribute Dependency { get; }

            public StateAttribute State { get; }

            public bool ResolverParameter => Dependency != null || State != null;

            public override string ToString()
            {
                return $"{Parameter.Name}";
            }
        }
    }
}
