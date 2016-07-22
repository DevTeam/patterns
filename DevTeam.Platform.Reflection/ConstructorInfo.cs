using System;

namespace DevTeam.Platform.Reflection
{
    internal class ConstructorInfo : MemberInfo, IConstructorInfo
    {
        private readonly global::System.Reflection.ConstructorInfo _constructorInfo;

        public ConstructorInfo(global::System.Reflection.ConstructorInfo constructorInfo)
            : base(constructorInfo)
        {
            if (constructorInfo == null) throw new ArgumentNullException(nameof(constructorInfo));

            _constructorInfo = constructorInfo;
        }
               
        public object Invoke(params object[] parameters)
        {
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            return _constructorInfo.Invoke(parameters);
        }
    }
}
