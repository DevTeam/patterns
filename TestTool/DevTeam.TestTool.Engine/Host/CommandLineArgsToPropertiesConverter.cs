namespace DevTeam.TestTool.Engine.Host
{
    using System.Collections.Generic;

    using Contracts;

    internal class CommandLineArgsToPropertiesConverter: IConverter<string[], IEnumerable<PropertyValue>>
    {
        public IEnumerable<PropertyValue> Convert(string[] source)
        {
            return new[] { new PropertyValue(ToolProperty.Shared, "explorer") };
        }
    }
}
