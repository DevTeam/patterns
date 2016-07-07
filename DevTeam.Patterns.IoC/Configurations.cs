namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;

    public static class Configurations
    {        
        public static IDisposable Apply(this IConfiguration configuration, IContainer container, IEqualityComparer<IConfiguration> comparer = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (container == null) throw new ArgumentNullException(nameof(container));

            return (
                from configurationItem in configuration.Merge(new HashSet<IConfiguration>(comparer ?? container.Resolve<IEqualityComparer<IConfiguration>>()))
                select configurationItem.CreateRegistrations(container))
                .SelectMany(i => i)
                .ToCompositeDisposable();
        }

        private static IEnumerable<IConfiguration> Merge(this IConfiguration configuration, HashSet<IConfiguration> configurations)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (configurations == null) throw new ArgumentNullException(nameof(configurations));

            if (!configurations.Add(configuration))
            {
                return configurations;
            }

            foreach (var dependency in configuration.GetDependencies())
            {
                dependency.Merge(configurations);
            }

            return configurations;
        }        
    }
}
