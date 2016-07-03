namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    public interface IAccuracy: IContext
    {
        IEnumerable<IKey> GetResolverKeys(KeyDescription keyDescription);
    }
}
