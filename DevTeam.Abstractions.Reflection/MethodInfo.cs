namespace DevTeam.Abstractions.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Abstractions;

    internal class MethodInfo: IMethodInfo
    {
        private readonly System.Reflection.MethodInfo _methodInfo;

        public MethodInfo(System.Reflection.MethodInfo methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));

            _methodInfo = methodInfo;
        }

        public string Name => _methodInfo.Name;

        public IEnumerable<T> GetCustomAttributes<T>() where T : Attribute
        {
            return _methodInfo.GetCustomAttributes<T>();
        }

        public object Invoke(object instance)
        {
            return _methodInfo.Invoke(instance, null);
        }
    }
}
