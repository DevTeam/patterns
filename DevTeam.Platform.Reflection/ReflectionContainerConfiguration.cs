namespace DevTeam.Platform.Reflection
{
    using System;
    using System.Collections.Generic;

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

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IReflection>(() => new Reflection(container.Resolver<System.Reflection.Assembly, IAssembly>()));
            yield return container.Register<System.Reflection.Assembly, IAssembly>(assembly => new Assembly(assembly, container.Resolver<System.Type, IType>()));
            yield return container.Register<System.Type, IType>(type => new Type(type, container.Resolver<System.Reflection.MethodInfo, IMethodInfo>()));
            yield return container.Register<System.Reflection.MethodInfo, IMethodInfo>(methodInfo => new MethodInfo(methodInfo));
        }
    }
}
