namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using System;

    internal class BindElement
    {
        public string State { get; set; }

        public string Instance { get; set; }

        public string Implementation { get; set; }

        public KeyElement Key { get; set; }
    }
}
