namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;

    public static class Configurations
    {
        public static IDisposable Apply(this IContainer container, params IConfiguration[] configurations)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (configurations == null) throw new ArgumentNullException(nameof(configurations));

            return container.Apply((IEnumerable<IConfiguration>)configurations);
        }

        public static IDisposable Apply(this IContainer container, IEnumerable<IConfiguration> configurations)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (configurations == null) throw new ArgumentNullException(nameof(configurations));

            return (
                from configuration in configurations
                select Apply(container, configuration)
            ).SelectMany(i => i).Distinct().ToCompositeDisposable();
        }

        private static IEnumerable<IDisposable> Apply(IContainer container, IConfiguration configuration)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            yield return container.Apply(configuration.GetDependencies());
            foreach (var configurationToken in configuration.Apply(container))
            {
                yield return configurationToken;
            }                       
        }
    }
}
