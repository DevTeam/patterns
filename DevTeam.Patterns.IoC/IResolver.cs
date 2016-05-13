namespace DevTeam.Patterns.IoC
{
	internal interface IResolver
	{
		T Resolve<T>(string name = "");

		T Resolve<TArg, T>(TArg arg, string name = "");
	}
}