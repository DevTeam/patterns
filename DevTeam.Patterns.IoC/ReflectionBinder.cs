﻿namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class ReflectionBinder: IBinder
    {
        public IRegistration Bind(IRegistry registry, Type stateType, Type contractType, Type implementationType, object key = null)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            ConstructorInfo resolvingConstructor;
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
            if (stateType != typeof(EmptyState))
            {
                var stateParamaters = (
                    from parameter in ctorParameters
                    where parameter.State != null
                    select parameter).ToList();

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

            return registry.Register(stateType, contractType,
                ctx =>
                {
                    var parameters = ctorParameters.Select(parameter => ResolveParameter(ctx.Resolver, ctx.State, parameter)).ToArray();
                    return resolvingConstructor.Invoke(parameters);
                },
                key);
        }

        private static object ResolveParameter(IResolver resolver, object state, CtorParameter parameter)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));

            if (parameter.State != null)
            {
                return state;
            }

            if (parameter.Dependency != null)
            {
                var dependency = parameter.Dependency;
                return resolver.Resolve(
                    resolver,
                    dependency.StateType ?? typeof(EmptyState),
                    dependency.ContractType ?? parameter.Parameter.ParameterType,
                    dependency.State ?? EmptyState.Shared,
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
                Dependency = parameter.GetCustomAttribute<DependencyAttribute>() ?? new DependencyAttribute();
                State = parameter.GetCustomAttribute<StateAttribute>();
            }

            public ParameterInfo Parameter { get; }

            public DependencyAttribute Dependency { get; }

            public StateAttribute State { get; }

            public override string ToString()
            {
                return $"{Parameter.Name}";
            }
        }
    }
}