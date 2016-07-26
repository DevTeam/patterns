namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using System.Collections.Generic;

    internal class ContainerBaseElemen
    {
        public IList<ContainerElement> Containers { get; set; }

        public IList<ScopeElement> Scopes { get; set; }
    }
}
