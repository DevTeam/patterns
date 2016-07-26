namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using System.Collections.Generic;

    internal class ConfigurationElement: ContainerBaseElemen
    {
        public IList<DependencyElement> Dependencies { get; set; }
    }
}
