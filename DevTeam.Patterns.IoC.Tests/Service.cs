namespace DevTeam.Patterns.IoC.Tests
{
	public interface IService
	{
	    object State { get; }

        IService Dependency { get; }
    }

    public interface IService2<T>
    {
    }

    class Service1 : IService
    {
        public object State { get; }

        public IService Dependency { get; }
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

        public IService Dependency { get; }
    }

    class Service1WithStateAndDependency : IService
    {
        private readonly string _state;
        private readonly IService _dependency;

        [Resolver]
        public Service1WithStateAndDependency([State] string state, [Dependency(InstanceType = typeof(IService), Key = "dep")] IService dependency)
        {
            _state = state;
            _dependency = dependency;
        }

        public object State => _state;

        public IService Dependency => _dependency;
    }

    class Service1WithStateAndDependencyFromAttr : IService
    {
        private readonly int _state;
        private readonly IService _dependency;

        [Resolver]
        public Service1WithStateAndDependencyFromAttr([State] int state, [Dependency(InstanceType = typeof(IService), Key = "dep2", StateType = typeof(string), State = "defaultState")] IService dependency)
        {
            _state = state;
            _dependency = dependency;
        }

        public object State => _state;

        public IService Dependency => _dependency;
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

        public IService Dependency => _dependency;
    }
}
