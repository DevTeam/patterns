namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    internal class ComparerForRegistrationComparer : IComparer<IComparer>
    {
        public int Compare(IComparer x, IComparer y)
        {
            return Comparer<WellknownComparer>.Default.Compare(x.Key, y.Key);
        }
    }
}