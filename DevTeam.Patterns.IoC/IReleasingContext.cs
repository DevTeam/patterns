namespace DevTeam.Patterns.IoC
{
    public interface IReleasingContext
    {
        IContainer Container { get; }

        IRegistration Registration { get; }

        string Name { get; }
    }
}