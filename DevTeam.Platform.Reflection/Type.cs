using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Reflection_ConstructorInfo = System.Reflection.ConstructorInfo;

namespace DevTeam.Platform.Reflection
{
    using Patterns.IoC;

    internal class Type: IType
    {
        private readonly global::System.Type _type;
        private readonly IResolver<global::System.Reflection.MethodInfo, IMethodInfo> _methodInfoResolver;
        private readonly IResolver<Reflection_ConstructorInfo, IConstructorInfo> _constructorInfoResolver;

        public Type(
            global::System.Type type,
            IResolver<global::System.Reflection.MethodInfo, IMethodInfo> methodInfoResolver,
            IResolver<global::System.Reflection.ConstructorInfo, IConstructorInfo> constructorInfoResolver)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (methodInfoResolver == null) throw new ArgumentNullException(nameof(methodInfoResolver));
            if (constructorInfoResolver == null) throw new ArgumentNullException(nameof(constructorInfoResolver));

            _type = type;
            _methodInfoResolver = methodInfoResolver;
            _constructorInfoResolver = constructorInfoResolver;
        }

        public string FullName => _type.FullName;

        public IEnumerable<IMethodInfo> Methods => _type.GetTypeInfo().DeclaredMethods.Select(i => _methodInfoResolver.Resolve(i));

        public IEnumerable<IConstructorInfo> Constructors => _type.GetTypeInfo().DeclaredConstructors.Select(i => _constructorInfoResolver.Resolve(i));

        public IEnumerable<T> GetCustomAttributes<T>()
            where T : Attribute
        {
            return _type.GetTypeInfo().GetCustomAttributes<T>();
        }

        public object CreateInstance()
        {
            return Activator.CreateInstance(_type);
        }
    }
}
