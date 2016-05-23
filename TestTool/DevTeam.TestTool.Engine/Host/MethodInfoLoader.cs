namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Reflection;

    using Contracts;

    internal class MethodInfoLoader : IMethodInfoLoader
    {
        public MethodInfo Load(Type type, string methodName)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));

            return type.GetRuntimeMethod(methodName, null);
        }
    }
}
