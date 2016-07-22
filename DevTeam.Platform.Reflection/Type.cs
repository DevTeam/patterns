﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DevTeam.Platform.Reflection
{
    using Patterns.IoC;

    internal class Type: IType
    {
        private readonly global::System.Type _type;
        private readonly IResolver<global::System.Reflection.MethodInfo, IMethodInfo> _methodInfoResolver;

        public Type(
            global::System.Type type,
            IResolver<global::System.Reflection.MethodInfo, IMethodInfo> methodInfoResolver)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (methodInfoResolver == null) throw new ArgumentNullException(nameof(methodInfoResolver));

            _type = type;
            _methodInfoResolver = methodInfoResolver;
        }

        public string FullName => _type.FullName;

        public IEnumerable<IMethodInfo> Methods => _type.GetTypeInfo().DeclaredMethods.Select(i => _methodInfoResolver.Resolve(i));

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
