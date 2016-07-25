namespace DevTeam.Patterns.IoC
{
    public interface IResolver<out T>
    {
        T Resolve(object key = null);
    }
}
