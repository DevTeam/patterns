namespace DevTeam.Patterns.IoC.v2
{
    public interface IResolving
    {
        IResolvingKey Key { get; }

        IContainer ResolvingContainer { get; }

        IContainer TargetContainer { get; }

        IRegistration Registration { get; }
    }
}
