﻿using System;
using System.Reflection;

namespace DevTeam.Platform.Reflection
{
    using Patterns.IoC;

    public class Reflection : IReflection
    {
        private readonly IResolver<global::System.Reflection.Assembly, IAssembly> _assemblyResolver;

        public Reflection(IResolver<global::System.Reflection.Assembly, IAssembly> assemblyResolver)
        {
            if (assemblyResolver == null) throw new ArgumentNullException(nameof(assemblyResolver));

            _assemblyResolver = assemblyResolver;
        }

        public IAssembly LoadAssembly(string assemblyFileName)
        {
            if (assemblyFileName == null) throw new ArgumentNullException(nameof(assemblyFileName));

            return _assemblyResolver.Resolve(global::System.Reflection.Assembly.Load(new AssemblyName(assemblyFileName)));
        }
    }
}
