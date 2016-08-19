namespace DevTeam.Patterns.IoC
{
    internal class FullComplianceComparer : IComparer
    {
        public WellknownComparer Key => WellknownComparer.FullCompliance;

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