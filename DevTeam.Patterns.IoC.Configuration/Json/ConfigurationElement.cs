namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using System.Collections.Generic;

    internal class ConfigurationElement
    {
        public IDictionary<string, string> Vars { get; set; }

        public IList<DependencyElement> Dependencies { get; set; }

        public IList<ContainerElement> Containers { get; set; }

        public IList<RegistrationElement> Registrations { get; set; }

        public override string ToString()
        {
            return $"{nameof(ConfigurationElement)} [{GetDesctiption()}]";
        }

        protected string GetDesctiption()
        {
            return $"Vars: {Vars}, Dependencies: {Dependencies}, Containers: {Containers}, Registrations: {Registrations}";
        }
    }
}
