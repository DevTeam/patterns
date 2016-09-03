namespace DevTeam.Patterns.IoC.v2
{
    public interface IContainer: IRegistry, IResolver
    {
        object Key { get; }
    }
}
