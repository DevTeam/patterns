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

        public object Create(ConstructorInfo constructor, params object[] parameters)
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

        private static ObjectActivator CreateActivator(ConstructorInfo ctor)
        {
            var args = Expression.Parameter(typeof(object[]), "args");
            var parameters = ctor.GetParameters().Select((parameter, index) => Expression.Convert(Expression.ArrayIndex(args, Expression.Constant(index)), parameter.ParameterType));
            return Expression.Lambda<ObjectActivator>(Expression.New(ctor, parameters), args).Compile();
        }
    }
}