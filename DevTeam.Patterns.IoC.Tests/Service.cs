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
        public Service1WithStateAndDependency([State] string state, [Dependency(typeof(IService), "dep")] IService dependency)
        {
            _state = state;
            _dependency = dependency;
        }

        public object State => _state;

        public IService Dependency => _dependency;
    }
}
