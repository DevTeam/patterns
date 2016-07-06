namespace DevTeam.Abstractions.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Patterns.IoC;

    internal class Assembly : IAssembly
    {
        private readonly System.Reflection.Assembly _assembly;
        private readonly IResolver<System.Type, IType> _typeResolver;

        public Assembly(
            System.Reflection.Assembly assembly,
            IResolver<System.Type, IType> typeResolver)
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