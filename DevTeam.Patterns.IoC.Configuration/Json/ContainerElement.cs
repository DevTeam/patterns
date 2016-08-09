namespace DevTeam.Patterns.IoC.Configuration.Json
{
    internal class ContainerElement: ConfigurationElement
    {
        public KeyElement Key { get; set; }

        public override string ToString()
        {
            return $"{nameof(ContainerElement)} [Key: {Key?.ToString() ?? "null"}, {GetDesctiption()}]";
        }
    }
}