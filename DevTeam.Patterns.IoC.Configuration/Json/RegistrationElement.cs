namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using Newtonsoft.Json;

    internal class RegistrationElement
    {
        public string State { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Contract { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Implementation { get; set; }

        public KeyElement Key { get; set; }

        public WellknownLifetime? Lifetime { get; set; }

        public WellknownRegistrationComparer? RegistrationComparer { get; set; }

        public WellknownScope? Scope { get; set; }

        public override string ToString()
        {
            return $"{nameof(RegistrationElement)} [State: {State ?? "null"}, Contract: {Contract ?? "null"}, Implementation: {Implementation ?? "null"}, Key: {Key?.ToString() ?? "null"}, Lifetime: {Lifetime?.ToString() ?? nameof(WellknownLifetime.Transient)}, RegistrationComparer: {RegistrationComparer?.ToString() ?? nameof(WellknownRegistrationComparer.FullCompliance)}, Scope: {Scope?.ToString() ?? nameof(WellknownScope.Public)}]";
        }
    }
}
