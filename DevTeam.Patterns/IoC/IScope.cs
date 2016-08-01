namespace DevTeam.Patterns.IoC
{
    public interface IScope: IContext
    {
        bool Satisfy(IResolver resolver);
    }
}
