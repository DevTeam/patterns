namespace DevTeam.Patterns.IoC
{
    public interface IResolver<in TState, out T>
    {
        T Resolve(TState state, string name = "");
    }
}
