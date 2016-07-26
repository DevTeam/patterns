namespace DevTeam.Patterns.IoC
{
    internal class RootContainerRegistrationComparer: IRegistrationComparer
    {
        public WellknownRegistrationComparer Key => WellknownRegistrationComparer.Default;

        public bool Equals(IRegistration x, IRegistration y)
        {
            if (x.ContractType == typeof(IContainer) && x.StateType == typeof(EmptyState)
                && y.ContractType == typeof(IContainer) && y.StateType == typeof(EmptyState))
            {
                return true;
            }

            return x.Equals(y);
        }

        public int GetHashCode(IRegistration obj)
        {
            if(obj.ContractType == typeof(IContainer) && obj.StateType == typeof(EmptyState))
            {
                unchecked
                {
                    var hashCode = obj.StateType.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.ContractType.GetHashCode();                        
                    return hashCode;
                }
            }

            return obj.GetHashCode();
        }
    }
}