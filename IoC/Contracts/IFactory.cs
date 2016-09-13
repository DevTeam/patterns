namespace IoC.Contracts
{
    public interface IFactory
    {
        object Create(IResolving resolving);
    }
}
