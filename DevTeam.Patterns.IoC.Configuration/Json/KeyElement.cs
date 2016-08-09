namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using Newtonsoft.Json;

    internal class KeyElement
    {
        public string Type { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{nameof(KeyElement)} [Type: {Type ?? nameof(System.String)}, Value: {Value ?? "null"}]";
        }
    }
}
