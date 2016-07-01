namespace DevTeam.Patterns.IoC.Tests
{
	public interface IService1
	{
	}

    public interface IService2<T>
    {
    }

    class Service1 : IService1
    {
    }
}
