namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    internal class PropertyFactory : IPropertyFactory
    {
        private readonly Dictionary<string, IProperty> _properties;

        public PropertyFactory(IEnumerable<IProperty> properties)
        {
            _properties = properties.ToDictionary(i => i.Id, i => i);
        }
        
        public IPropertyValue CreatePropertyValue(string propertyId, string propertyValue)
        {
            IProperty property;
            if (!_properties.TryGetValue(propertyId, out property))
            {
                throw new InvalidOperationException($"Invalid property \"{propertyId}\".");
            }

            if (!property.Validate(propertyValue))
            {
                throw new InvalidOperationException($"Invalid value \"{propertyValue}\" for property {property}.");
            }

            return new PropertyValue(property, propertyValue);
        }
    }
}
