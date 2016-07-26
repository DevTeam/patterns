﻿namespace DevTeam.Patterns.IoC
{
    internal class AnyKeyRegistrationComparer: IRegistrationComparer
    {
        public WellknownRegistrationComparer Key => WellknownRegistrationComparer.AnyKey;

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
