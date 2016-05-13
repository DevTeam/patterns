namespace DevTeam.Patterns.IoC
{
	internal interface IConfiguration
	{
		IContainer Apply(IContainer container);
	}
}