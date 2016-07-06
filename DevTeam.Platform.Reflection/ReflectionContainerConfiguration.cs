namespace DevTeam.Platform.Reflection
{
    using System;
    using System.Collections.Generic;

    using Patterns.Dispose;
    using Patterns.IoC;

    public class ReflectionContainerConfiguration : IConfiguration
    {
        public static readonly IConfiguration Shared = new ReflectionContainerConfiguration();

        private ReflectionContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IReflection>(() => new Reflection(container.Resolver<System.Reflection.Assembly, IAssembly>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<System.Reflection.Assembly, IAssembly>(assembly => new Assembly(assembly, container.Resolver<System.Type, IType>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<System.Type, IType>(type => new Type(type, container.Resolver<System.Reflection.MethodInfo, IMethodInfo>()));
            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<System.Reflection.MethodInfo, IMethodInfo>(methodInfo => new MethodInfo(methodInfo));
        }
    }
}
