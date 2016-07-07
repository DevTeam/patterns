namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    using Patterns.IoC;

    internal class PropertyFactory : IPropertyFactory
    {
        private readonly IResolver<PropertyValueDescription, IPropertyValue> _propertyResolver;
        private readonly Dictionary<string, IProperty> _properties;

        public PropertyFactory(
            IEnumerable<IProperty> properties,
            IResolver<PropertyValueDescription, IPropertyValue> propertyResolver)
        {            
            if (properties == null) throw new ArgumentNullException(nameof(properties));
            if (propertyResolver == null) throw new ArgumentNullException(nameof(propertyResolver));

            _propertyResolver = propertyResolver;
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

            return _propertyResolver.Resolve(new PropertyValueDescription(property, propertyValue));
        }
    }
}
