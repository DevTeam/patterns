namespace DevTeam.Patterns.IoC
{
    internal class RootContainerRegestryKeyComparer: IRegistryKeyComparer
    {
        public bool Equals(IRegestryKey x, IRegestryKey y)
        {
            if (x.InstanceType == typeof(IContainer) && x.StateType == typeof(EmptyState)
                && y.InstanceType == typeof(IContainer) && y.StateType == typeof(EmptyState))
            {
                return true;
            }

            return x.Equals(y);
        }

        public int GetHashCode(IRegestryKey obj)
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