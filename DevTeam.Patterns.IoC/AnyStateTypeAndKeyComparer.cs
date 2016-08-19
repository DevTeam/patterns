namespace DevTeam.Patterns.IoC
{
    internal class AnyStateTypeAndKeyComparer : IComparer
    {
        public WellknownComparer Key => WellknownComparer.AnyStateTypeAndKey;

        public bool Equals(IRegistration x, IRegistration y)
        {
            return x.ContractType == y.ContractType;
        }

        public int GetHashCode(IRegistration obj)
        {
            return obj.ContractType.GetHashCode();
        }
    }
}
