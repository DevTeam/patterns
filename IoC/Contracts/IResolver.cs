namespace IoC.Contracts
{
    public interface IResolver
    {
        bool TryResolve(out object value, IKey key, params object[] states);
    }
}
