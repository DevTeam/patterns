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

        public WellknownComparer? Comparer { get; set; }

        public WellknownScope? Scope { get; set; }

        public WellknownContractRange? ContractRange { get; set; }

        public override string ToString()
        {
            return $"{nameof(RegistrationElement)} [State: {State ?? "null"}, Contract: {Contract ?? "null"}, Implementation: {Implementation ?? "null"}, Key: {Key?.ToString() ?? "null"}, Lifetime: {Lifetime?.ToString() ?? nameof(WellknownLifetime.Transient)}, RegistrationComparer: {Comparer?.ToString() ?? nameof(WellknownComparer.FullCompliance)}, Scope: {Scope?.ToString() ?? nameof(WellknownScope.Public)}, ContractRange: {ContractRange?.ToString() ?? nameof(WellknownContractRange.Contract)}]";
        }
    }
}
