using System;
using System.Collections.Generic;
using System.Reflection;

namespace DevTeam.Platform.Reflection
{
    internal class MemberInfo: IMemberInfo
    {
        private readonly global::System.Reflection.MemberInfo _memberInfo;

        public MemberInfo(global::System.Reflection.MemberInfo memberInfo)
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));

            _memberInfo = memberInfo;
        }

        public string Name => _memberInfo.Name;

        public IEnumerable<T> GetCustomAttributes<T>() where T : Attribute
        {
            return _memberInfo.GetCustomAttributes<T>();
        }        
    }
}
