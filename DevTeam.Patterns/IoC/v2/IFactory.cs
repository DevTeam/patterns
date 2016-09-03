namespace DevTeam.Patterns.IoC.v2
{
    public interface IFactory
    {
        object Create(IResolving resolving);
    }
}
