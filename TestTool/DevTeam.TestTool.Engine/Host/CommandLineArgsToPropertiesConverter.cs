namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Contracts;

    internal class CommandLineArgsToPropertiesConverter: IConverter<string[], IEnumerable<IPropertyValue>>
    {
        private static readonly Regex PropertyRegex = new Regex("-(?<name>.+)=(?<value>.+)", RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private readonly IPropertyFactory _propertyFactory;

        public CommandLineArgsToPropertiesConverter(
            IPropertyFactory propertyFactory)
        {
            if (propertyFactory == null) throw new ArgumentNullException(nameof(propertyFactory));

            _propertyFactory = propertyFactory;
        }

        public IEnumerable<IPropertyValue> Convert(string[] source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return 
                from propertyStr in source
                let propertyMath = PropertyRegex.Match(propertyStr)
                where propertyMath.Success
                select _propertyFactory.CreatePropertyValue(propertyMath.Groups["name"].Value, propertyMath.Groups["value"].Value);
        }
    }
}
