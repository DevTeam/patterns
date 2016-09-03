namespace DevTeam.Patterns.IoC.v2
{
    public interface IResolver
    {
        object Resolve(IResolvingKey key);
    }
}
