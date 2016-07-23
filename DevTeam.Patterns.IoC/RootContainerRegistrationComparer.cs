namespace DevTeam.Patterns.IoC
{
    internal class RootContainerRegistrationComparer: IRegistrationComparer
    {
        public WellknownRegistrationComparer Key => WellknownRegistrationComparer.Default;

        public bool Equals(IRegistration x, IRegistration y)
        {
            if (x.InstanceType == typeof(IContainer) && x.StateType == typeof(EmptyState)
                && y.InstanceType == typeof(IContainer) && y.StateType == typeof(EmptyState))
            {
                return true;
            }

            return x.Equals(y);
        }

        public int GetHashCode(IRegistration obj)
        {
            if(obj.InstanceType == typeof(IContainer) && obj.StateType == typeof(EmptyState))
            {
                unchecked
                {
                    var hashCode = obj.StateType.GetHashCode();
                    hashCode = (hashCode * 397) ^ obj.InstanceType.GetHashCode();                        
                    return hashCode;
                }
            }

            return obj.GetHashCode();
        }
    }
}