namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    public interface IRegistryKeyComparer: IEqualityComparer<IRegestryKey>, IRegisteryContext
    {
    }
}
