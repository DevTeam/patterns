namespace DevTeam.Patterns.IoC
{
	public interface IConfiguration
	{
		IContainer Apply(IContainer container);
	}
}