namespace DevTeam.Patterns.IoC
{
    public interface IContainer : IResolver, IRegistry
    {
        string Name { get; }
    }
}