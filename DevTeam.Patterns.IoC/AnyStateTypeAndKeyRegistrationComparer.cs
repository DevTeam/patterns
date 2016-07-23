namespace DevTeam.Patterns.IoC
{
    internal class AnyStateTypeAndKeyRegistrationComparer: IRegistrationComparer
    {
        public WellknownRegistrationComparer Key => WellknownRegistrationComparer.AnyStateTypeAndKey;

        public bool Equals(IRegistration x, IRegistration y)
        {
            return x.InstanceType == y.InstanceType;
        }

        public int GetHashCode(IRegistration obj)
        {
            return obj.InstanceType.GetHashCode();
        }
    }
}
