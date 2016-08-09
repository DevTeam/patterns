namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using Newtonsoft.Json;

    internal class DependencyElement
    {
        [JsonProperty(Required = Required.Always)]
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{nameof(DependencyElement)} [Type: {Type ?? nameof(System.String)}]";
        }
    }
}
