using System;

namespace DevTeam.Platform.Reflection
{
    internal class MethodInfo: MemberInfo, IMethodInfo
    {
        private readonly global::System.Reflection.MethodInfo _methodInfo;

        public MethodInfo(global::System.Reflection.MethodInfo methodInfo)
            : base(methodInfo)
        {
            if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));

            _methodInfo = methodInfo;
        }       

        public object Invoke(object instance, object[] parameters)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            return _methodInfo.Invoke(instance, parameters);
        }
    }
}
