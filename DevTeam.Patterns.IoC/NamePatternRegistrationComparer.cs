namespace DevTeam.Patterns.IoC
{
    using System.Text.RegularExpressions;

    internal class NamePatternRegistrationComparer: IRegistrationComparer
    {
        public bool Equals(IRegistration x, IRegistration y)
        {
            var xName = x.Name?.ToString() ?? string.Empty;
            var yName = y.Name?.ToString() ?? string.Empty;
            var regexX = new Regex(xName);
            var regexY = new Regex(yName);
            if (x.InstanceType == y.InstanceType && x.StateType == y.StateType && (regexX.IsMatch(yName) || regexY.IsMatch(xName)))
            {
                return true;
            }

            return x.Equals(y);
        }

        public int GetHashCode(IRegistration obj)
        {
            unchecked
            {
                var hashCode = obj.StateType.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.InstanceType.GetHashCode();
                return hashCode;
            }
        }
    }
}
