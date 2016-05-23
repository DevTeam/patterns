namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;

    using Contracts;

    internal class CommandLineArgsToPropertiesConverter: IConverter<string[], IEnumerable<PropertyValue>>
    {
        public IEnumerable<PropertyValue> Convert(string[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return new[] { new PropertyValue(ToolProperty.Shared, "explorer"), new PropertyValue(AssemblyProperty.Shared, @"C:\Projects\GitHub\patterns\TestTool\DevTeam.TestTool.dotNet\bin\Debug\DevTeam.TestTool.Test.Mocks.dll, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null") };
        }
    }
}
