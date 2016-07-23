namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    internal class ComparerForRegistrationComparer : IComparer<IRegistrationComparer>
    {
        public int Compare(IRegistrationComparer x, IRegistrationComparer y)
        {
            return Comparer<WellknownRegistrationComparer>.Default.Compare(x.Key, y.Key);
        }
    }
}