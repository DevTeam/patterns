namespace DevTeam.Patterns.IoC
{
    public interface IScope : IContext
    {
        bool ReadyToRegister(bool isRoot, IContainer container);

        bool ReadyToResolve(bool isRoot, IContainer container);
    }
}
