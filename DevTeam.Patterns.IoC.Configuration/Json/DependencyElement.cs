namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using Newtonsoft.Json;

    internal class DependencyElement
    {
        [JsonProperty(Required = Required.Always)]
        public string Type { get; set; }
    }
}
