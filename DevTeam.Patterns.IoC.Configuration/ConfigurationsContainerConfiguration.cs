namespace DevTeam.Patterns.IoC.Configuration
{
    using System.Collections.Generic;

    using Json;

    public class ConfigurationsContainerConfiguration: IConfiguration
    {
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            yield return container.Register<JsonConfiguration>().As<string, IConfiguration>(WellknownConfigurations.Json);
        }
    }
}