namespace DevTeam.Patterns.IoC
{
    internal class DefaultContainerRegistrationComparer: IRegistrationComparer
    {
        public WellknownRegistrationComparer Key => WellknownRegistrationComparer.FullCompliance;

        public bool Equals(IRegistration x, IRegistration y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(IRegistration obj)
        {
            return obj.GetHashCode();
        }
    }
}