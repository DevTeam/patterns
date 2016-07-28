namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ExpressionFactory: IFactory
    {
        private delegate object ObjectActivator(params object[] args);

        private readonly Dictionary<ConstructorInfo, ObjectActivator> _activators = new Dictionary<ConstructorInfo, ObjectActivator>();

        public object Create(ConstructorInfo constructor, object[] parameters)
        {
            if (constructor == null) throw new ArgumentNullException(nameof(constructor));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            ObjectActivator activator;
            if (!_activators.TryGetValue(constructor, out activator))
            {
                activator = CreateActivator(constructor);
                _activators.Add(constructor, activator);
            }

            return activator(parameters);
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

        private static ObjectActivator CreateActivator(ConstructorInfo ctor)
        {
            var args = Expression.Parameter(typeof(object[]), "args");
            var parameters = ctor.GetParameters().Select((parameter, index) => Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(index)), parameter.ParameterType));
            return Expression.Lambda<ObjectActivator>(Expression.New(ctor, parameters), args).Compile();
        }
    }
}