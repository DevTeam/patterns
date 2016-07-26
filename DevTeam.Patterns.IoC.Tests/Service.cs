namespace DevTeam.Patterns.IoC.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IService
	{
	    object State { get; }

        object Dependency { get; }
    }

    public interface IService2<T>
    {
    }

    class Service1 : IService
    {
        public object State { get; }

        public object Dependency { get; }
    }

    class Service1WithState : IService
    {
        private readonly string _state;

        [Resolver]
        public Service1WithState([State] string state)
        {
            _state = state;
        }

        public object State => _state;

        public object Dependency { get; }
    }

    class Service1WithStateAndDependency : IService
    {
        private readonly string _state;
        private readonly IService _dependency;

        [Resolver]
        public Service1WithStateAndDependency([State] string state, [Dependency(ContractType = typeof(IService), Key = "dep")] IService dependency)
        {
            _state = state;
            _dependency = dependency;
        }

        public object State => _state;

        public object Dependency => _dependency;
    }

    class Service1WithStateAndNotMarkedDependency : IService
    {
        private readonly string _state;
        private readonly int _dependency;

        [Resolver]
        public Service1WithStateAndNotMarkedDependency([State] string state, int dependency)
        {
            _state = state;
            _dependency = dependency;
        }

        public object State => _state;

        public object Dependency => _dependency;
    } 

    class Service1WithStateAndDependencyFromAttr : IService
    {
        private readonly int _state;
        private readonly IService _dependency;

        [Resolver]
        public Service1WithStateAndDependencyFromAttr([State] int state, [Dependency(ContractType = typeof(IService), Key = "dep2", StateType = typeof(string), State = "defaultState")] IService dependency)
        {
            _state = state;
            _dependency = dependency;
        }

        public object State => _state;

        public object Dependency => _dependency;
    }

    class Service1WithStateAndDependencyViaResolver : IService
    {
        private readonly int _state;
        private readonly IService _dependency;

        [Resolver]
        public Service1WithStateAndDependencyViaResolver([State] int state, [Dependency(Key = "dep2")] IResolver<string, IService> dependencyResolver)
        {
            _state = state;
            _dependency = dependencyResolver.Resolve("resolverState");
        }

        public object State => _state;

        public object Dependency => _dependency;
    }

    class Service1WithStateAndEnumerableDependency: IService
    {
        private readonly int _state;
        private readonly IEnumerable<IService> _dependency;

        [Resolver]
        public Service1WithStateAndEnumerableDependency([State] int state, [Dependency] IEnumerable<IService> dependency)
        {
            _state = state;
            _dependency = dependency.ToList();
        }

        public object State => _state;

        public object Dependency => _dependency;
    }
}
