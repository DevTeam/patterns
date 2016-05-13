namespace DevTeam.Patterns.IoC
{
    public interface IResolver
	{
		T Resolve<T>(string name = "");

		T Resolve<TArg, T>(TArg arg, string name = "");
	}
}