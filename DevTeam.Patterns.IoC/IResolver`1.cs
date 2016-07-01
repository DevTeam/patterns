namespace DevTeam.Patterns.IoC
{
    public interface IResolver<out T>
    {
        T Resolve(string name = "");
    }
}
