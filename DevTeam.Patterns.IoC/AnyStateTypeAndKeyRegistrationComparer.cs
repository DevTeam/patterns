namespace DevTeam.Patterns.IoC
{
    internal class AnyStateTypeAndKeyRegistrationComparer: IRegistrationComparer
    {
        public WellknownRegistrationComparer Key => WellknownRegistrationComparer.AnyStateTypeAndKey;

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
