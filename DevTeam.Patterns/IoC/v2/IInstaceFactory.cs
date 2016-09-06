namespace DevTeam.Patterns.IoC.v2
{
    public interface IInstaceFactory
    {
        object Create(IResolving resolving);
    }
}
