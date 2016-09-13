namespace IoC.Contracts
{
    public interface IStateFactory
    {
        object Create(IStateKey stateKey, IResolving resolving);
    }
}
