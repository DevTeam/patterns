namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using System.Collections.Generic;

    internal class ScopeElement
    {
        public IList<UsingElement> Using { get; set; }

        public IList<BindElement> Binds { get; set; }
    }
}
