using System;
using System.Collections.Generic;

namespace DevTeam.Platform.Reflection
{    
    using Patterns.IoC;

    /// <inheritdoc/>
    public class ReflectionContainerConfiguration : IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IReflection>(() => new Reflection(container.Resolver<global::System.Reflection.Assembly, IAssembly>(), container.Resolver<global::System.Type, IType>()));
            yield return container.Register<global::System.Reflection.Assembly, IAssembly>(assembly => new Assembly(assembly, container.Resolver<global::System.Type, IType>()));
            yield return container.Register<global::System.Type, IType>(type => new Type(type, container.Resolver<global::System.Reflection.MethodInfo, IMethodInfo>(), container.Resolver<global::System.Reflection.ConstructorInfo, IConstructorInfo>()));
            yield return container.Register<global::System.Reflection.MethodInfo, IMethodInfo>(methodInfo => new MethodInfo(methodInfo));
            yield return container.Register<global::System.Reflection.ConstructorInfo, IConstructorInfo>(constructorInfo => new ConstructorInfo(constructorInfo));
        }
    }
}
