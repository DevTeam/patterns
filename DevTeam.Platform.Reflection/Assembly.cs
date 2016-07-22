using System;
using System.Collections.Generic;
using System.Linq;

namespace DevTeam.Platform.Reflection
{
    using Patterns.IoC;

    internal class Assembly : IAssembly
    {
        private readonly global::System.Reflection.Assembly _assembly;
        private readonly IResolver<global::System.Type, IType> _typeResolver;

        public Assembly(
            global::System.Reflection.Assembly assembly,
            IResolver<global::System.Type, IType> typeResolver)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (typeResolver == null) throw new ArgumentNullException(nameof(typeResolver));

            _assembly = assembly;
            _typeResolver = typeResolver;
        }

        public IEnumerable<IType> DefinedTypes => _assembly.DefinedTypes.Select(i => _typeResolver.Resolve(i.AsType()));

        public IType GetType(string typeName)
        {
            return _typeResolver.Resolve(_assembly.GetType(typeName));
        }
    }
}