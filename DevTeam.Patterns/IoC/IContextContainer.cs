namespace DevTeam.Patterns.IoC
{
    public interface IContextContainer<TContext> : IContainer
        where TContext: IContext
    {
    }
}
