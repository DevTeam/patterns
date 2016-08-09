﻿namespace DevTeam.Patterns.IoC
{
    using System.Text.RegularExpressions;

    internal class PatternKeyRegistrationComparer : IRegistrationComparer
    {
        public WellknownRegistrationComparer Key => WellknownRegistrationComparer.PatternKey;

        public bool Equals(IRegistration x, IRegistration y)
        {
            var xKey = x.Key?.ToString() ?? string.Empty;
            var yKey = y.Key?.ToString() ?? string.Empty;
            var regexX = new Regex(xKey);
            var regexY = new Regex(yKey);
            if (x.ContractType == y.ContractType && x.StateType == y.StateType && (regexX.IsMatch(yKey) || regexY.IsMatch(xKey)))
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
                hashCode = (hashCode * 397) ^ obj.ContractType.GetHashCode();
                return hashCode;
            }
        }
    }
}
