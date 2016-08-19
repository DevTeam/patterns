namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    public interface IComparer : IEqualityComparer<IRegistration>, IContext
    {
        WellknownComparer Key { get; }
    }
}
