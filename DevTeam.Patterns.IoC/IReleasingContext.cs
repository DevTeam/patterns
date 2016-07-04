namespace DevTeam.Patterns.IoC
{
    public interface IReleasingContext
    {
        IContainer Container { get; }
        IRegestryKey RegestryKey { get; }
        string Name { get; }
    }
}