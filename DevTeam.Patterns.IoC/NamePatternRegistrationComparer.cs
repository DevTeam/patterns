namespace DevTeam.Patterns.IoC
{
    using System.Text.RegularExpressions;

    internal class NamePatternRegistrationComparer: IRegistrationComparer
    {
        public bool Equals(IRegistration x, IRegistration y)
        {
            var xKey = x.Key?.ToString() ?? string.Empty;
            var yKey = y.Key?.ToString() ?? string.Empty;
            var regexX = new Regex(xKey);
            var regexY = new Regex(yKey);
            if (x.InstanceType == y.InstanceType && x.StateType == y.StateType && (regexX.IsMatch(yKey) || regexY.IsMatch(xKey)))
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
