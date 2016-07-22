using System;
using System.Reflection;

namespace DevTeam.Platform.Reflection
{
    using Patterns.IoC;

    public class Reflection : IReflection
    {
        private readonly IResolver<global::System.Reflection.Assembly, IAssembly> _assemblyResolver;
        private readonly IResolver<global::System.Type, IType> _typeResolver;

        public Reflection(
            IResolver<global::System.Reflection.Assembly, IAssembly> assemblyResolver,
            IResolver<global::System.Type, IType> typeResolver)
        {
            if (assemblyResolver == null) throw new ArgumentNullException(nameof(assemblyResolver));
            if (typeResolver == null) throw new ArgumentNullException(nameof(typeResolver));

            _assemblyResolver = assemblyResolver;
            _typeResolver = typeResolver;
        }

        public IAssembly LoadAssembly(string assemblyFileName)
        {
            if (assemblyFileName == null) throw new ArgumentNullException(nameof(assemblyFileName));

            return _assemblyResolver.Resolve(global::System.Reflection.Assembly.Load(new AssemblyName(assemblyFileName)));
        }

        public IType GetType(string typeName, bool throwOnError)
        {
            if (typeName == null) throw new ArgumentNullException(nameof(typeName));

            return _typeResolver.Resolve(global::System.Type.GetType(typeName, throwOnError));
        }
    }
}
