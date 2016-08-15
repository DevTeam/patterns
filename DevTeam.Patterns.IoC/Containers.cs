namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Containers
    {
        private static readonly IEqualityComparer<IConfiguration> Comparer = new ConfigurationEqualityComparer();

        public static IContainer Create(object key = null)
        {
            return new Container(key);
        }

        public static IEnumerable<IRegistration> Apply(this IContainer container, IConfiguration configuration)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            return
                 GetDependencies(configuration)
                 .Concat(Enumerable.Repeat(configuration, 1))
                 .Distinct(Comparer)
                 .Select(config => config.CreateRegistrations(container))
                 .SelectMany(i => i);
        }

        private static IEnumerable<IConfiguration> GetDependencies(IConfiguration configuration)
        {
            return (
                from nestedConfiguration in configuration.GetDependencies()
                select GetDependencies(nestedConfiguration))
                .SelectMany(i => i).Concat(Enumerable.Repeat(configuration, 1));
        }

        private class ConfigurationEqualityComparer : IEqualityComparer<IConfiguration>
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
}
