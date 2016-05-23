namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class PropertyValue
    {
        public PropertyValue(IProperty property, string value)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (value == null) throw new ArgumentNullException(nameof(value));

            Property = property;
            Value = value;
        }

        public IProperty Property { get; private set; }

        public string Value { get; private set; }
    }
}
