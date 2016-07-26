namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using System.Collections.Generic;

    internal class ConfigurationElement
    {
        public IList<DependencyElement> Dependencies { get; set; }

        public IList<ContainerElement> Containers { get; set; }

        public IList<BindElement> Bindings { get; set; }
    }
}
