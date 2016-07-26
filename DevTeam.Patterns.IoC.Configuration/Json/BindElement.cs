namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using Newtonsoft.Json;

    internal class BindElement
    {
        public string State { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Contract { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Implementation { get; set; }

        public KeyElement Key { get; set; }

        public WellknownLifetime? Lifetime { get; set; }

        public WellknownRegistrationComparer? RegistrationComparer { get; set; }
    }
}
