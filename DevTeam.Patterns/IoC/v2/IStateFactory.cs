namespace DevTeam.Patterns.IoC.v2
{
    public interface IStateFactory
    {
        object Create(IStateKey stateKey, IResolving resolving);
    }
}
