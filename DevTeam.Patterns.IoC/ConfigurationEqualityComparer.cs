namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    internal class ConfigurationEqualityComparer: IEqualityComparer<IConfiguration>
    {
        public bool Equals(IConfiguration x, IConfiguration y)
        {
            return x?.GetType() == y?.GetType();            
        }

        public int GetHashCode(IConfiguration obj)
        {
            return obj?.GetType().GetHashCode() ?? 0;
        }
    }
}
