namespace DevTeam.Patterns.IoC
{
    using System.Text.RegularExpressions;

    internal class NamePatternRegestryKeyComparer: IRegistryKeyComparer
    {
        public bool Equals(IRegestryKey x, IRegestryKey y)
        {
            var regexX = new Regex(x.Name);
            var regexY = new Regex(y.Name);
            if (x.InstanceType == y.InstanceType && x.StateType == y.StateType && (regexX.IsMatch(y.Name) || regexY.IsMatch(x.Name)))
            {
                return true;
            }

            return x.Equals(y);
        }

        public int GetHashCode(IRegestryKey obj)
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
