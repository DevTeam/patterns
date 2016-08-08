namespace DevTeam.Patterns.IoC
{
    public interface IScope: IContext
    {
        bool ReadyToRegister(bool isRoot);

        bool ReadyToResolve(bool isRoot, IResolver resolver);
    }
}
