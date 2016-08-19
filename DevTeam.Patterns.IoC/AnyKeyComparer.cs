namespace DevTeam.Patterns.IoC
{
    internal class AnyKeyComparer : IComparer
    {
        public WellknownComparer Key => WellknownComparer.AnyKey;

        public bool Equals(IRegistration x, IRegistration y)
        {
            return x.ContractType == y.ContractType && x.StateType == y.StateType;
        }

        public int GetHashCode(IRegistration obj)
        {
            unchecked
            {
                var hashCode = obj.StateType.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.ContractType.GetHashCode();
                return hashCode;
            }
        }
    }
}
