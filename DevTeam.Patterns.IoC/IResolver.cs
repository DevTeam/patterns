namespace DevTeam.Patterns.IoC
{
    public interface IResolver
	{
		T Resolve<TState, T>(TState state, string name = "");
	}
}