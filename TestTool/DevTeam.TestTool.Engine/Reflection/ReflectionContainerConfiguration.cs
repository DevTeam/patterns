namespace DevTeam.TestTool.Engine.Reflection
{
    using System;

    using Contracts;

    using Patterns.Dispose;
    using Patterns.IoC;

    public class ReflectionContainerConfiguration : IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IReflection>(() => new Reflection(container.Resolver<System.Reflection.Assembly, IAssembly>())));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<System.Reflection.Assembly, IAssembly>(assembly => new Assembly(assembly, container.Resolver<System.Type, IType>())));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<System.Type, IType>(type => new Type(type, container.Resolver<System.Reflection.MethodInfo, IMethodInfo>())));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<System.Reflection.MethodInfo, IMethodInfo>(methodInfo => new MethodInfo(methodInfo)));

            return disposable;
        }
    }
}
