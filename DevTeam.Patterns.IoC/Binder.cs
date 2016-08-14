namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class Binder : IBinder
    {
        private readonly Dictionary<Type, CtorInfo> _ctorDictionary = new Dictionary<Type, CtorInfo>();

        public IRegistration Bind(IRegistry registry, Type stateType, Type contractType, Type implementationType, IFactory factory, object key = null)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            CheckConstraints(contractType, implementationType);
            var isGeneric = IsGeneric(implementationType);
            CtorInfo ctorInfo = null;
            if (!isGeneric)
            {
                ctorInfo = GetCtorInfo(stateType, implementationType);
            }

            return registry.Register(stateType, contractType, ctx => CreateInstance(ctorInfo, isGeneric, stateType, implementationType, factory, ctx), key);
        }

        private static void CheckConstraints(Type contractType, Type implementationType)
        {
            if (!IsGeneric(implementationType))
            {
                return;
            }

            if (IsGeneric(contractType))
            {
                throw new InvalidOperationException("A generic implementation should relay on a generic contract.");
            }

            var contractTypeInfo = contractType.GetTypeInfo();
            var implementationTypeInfo = implementationType.GetTypeInfo();
            if (contractTypeInfo.GenericTypeParameters.Length != implementationTypeInfo.GenericTypeParameters.Length)
            {
                throw new InvalidOperationException("A generic type parameters of implementation should correspond to generic type parameters of contract.");
            }
        }

        private object CreateInstance(CtorInfo ctorInfo, bool isGeneric, Type stateType, Type implementationType, IFactory factory, IResolvingContext resolvingContext)
        {
            if (isGeneric)
            {
                implementationType = implementationType.MakeGenericType(resolvingContext.ResolvingContractType.GenericTypeArguments);
                ctorInfo = GetCtorInfo(stateType, implementationType);
            }

            var resolvedParameters =
                from parameter in ctorInfo.Parameters
                select ResolveParameter(resolvingContext.RegisterContainer, resolvingContext.State, parameter);

            return factory.Create(ctorInfo.Constructor, resolvedParameters.ToArray());
        }

        private static bool IsGeneric(Type type)
        {
            return type.GetTypeInfo().IsGenericType && !type.IsConstructedGenericType;
        }

        private CtorInfo GetCtorInfo(Type stateType, Type implementationType)
        {
            CtorInfo ctorInfo;
            if (!_ctorDictionary.TryGetValue(implementationType, out ctorInfo))
            {
                ctorInfo = new CtorInfo(stateType, implementationType);
                _ctorDictionary.Add(implementationType, ctorInfo);
            }

            if (ctorInfo.Error != null)
            {
                throw ctorInfo.Error;
            }

            return ctorInfo;
        }

        private static object ResolveParameter(IResolver resolver, object state, CtorParameter parameter)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            if (parameter.State != null)
            {
                return ResolveState(resolver, parameter.Parameter, parameter.State, state);
            }

            if (parameter.Dependency != null)
            {
                var dependency = parameter.Dependency;
                return ResolveDependency(resolver, parameter.Parameter, dependency);
            }

            throw new InvalidOperationException("Fail to resolve");
        }

        private static string CreateParametersList(IEnumerable<CtorParameter> parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            return string.Join(", ", parameters.Select(i => $"\"{i}\""));
        }

        private static object ResolveState(IResolver resolver, ParameterInfo parameter, StateAttribute stateAttr, object state)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (stateAttr == null) throw new ArgumentNullException(nameof(stateAttr));

            return state;
        }

        private static object ResolveDependency(IResolver resolver, ParameterInfo parameter, DependencyAttribute dependencyAttr)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            if (dependencyAttr == null) throw new ArgumentNullException(nameof(dependencyAttr));

            return resolver.Resolve(
                dependencyAttr.StateType ?? typeof(EmptyState),
                dependencyAttr.ContractType ?? parameter.ParameterType,
                dependencyAttr.State ?? EmptyState.Shared,
                dependencyAttr.Key);
        }

        private class CtorInfo
        {
            public CtorInfo(Type stateType, Type implementationType)
            {
                Constructor = GetConstructor(implementationType);
                if (Constructor == null)
                {
                    return;
                }

                Parameters = Constructor.GetParameters().Select(parameter => new CtorParameter(parameter)).ToList();
                if (stateType != typeof(EmptyState))
                {
                    var stateParamaters = (
                        from parameter in Parameters
                        where parameter.State != null
                        select parameter).ToList();

                    if (stateParamaters.Count != 1)
                    {
                        Error = new InvalidOperationException($"Constructor should have only one state parameter, but has more: {CreateParametersList(stateParamaters)}");
                        return;
                    }

                    var stateParamater = stateParamaters.Single();
                    if (stateParamater.Parameter.ParameterType != stateType)
                    {
                        Error = new InvalidOperationException($"State parameter \"{stateParamater.Parameter.Name}\" has invalid type \"{stateParamater.Parameter.ParameterType.Name}\", but should have \"{stateType}\".");
                    }
                }
            }

            public ConstructorInfo Constructor { get; }

            public List<CtorParameter> Parameters { get; }

            public InvalidOperationException Error { get; private set; }

            private ConstructorInfo GetConstructor(Type implementationType)
            {
                var implementationTypeInfo = implementationType.GetTypeInfo();
                var ctorCount = implementationTypeInfo.DeclaredConstructors.Count();
                if (ctorCount == 1)
                {
                    return implementationTypeInfo.DeclaredConstructors.First();
                }

                try
                {
                    var resolvingConstructor = (
                        from ctor in implementationTypeInfo.DeclaredConstructors
                        let resolverAttribute = ctor.GetCustomAttribute<ResolverAttribute>()
                        where resolverAttribute != null
                        select ctor).SingleOrDefault();

                    if (resolvingConstructor != null)
                    {
                        return resolvingConstructor;
                    }
                }
                catch (InvalidOperationException)
                {
                    Error = new InvalidOperationException("Too many resolving constructors.");
                }


                Error = new InvalidOperationException("Resolving constructor was not found.");
                return null;
            }
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
