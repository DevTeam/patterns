namespace DevTeam.TestTool.Engine.Host
{
    using Contracts;

    using Patterns.IoC;

    internal class PropertyValue : PropertyValueDescription, IPropertyValue
    {
        public PropertyValue([State] PropertyValueDescription description)
            :base(description.Property, description.Value)
        {
        }
    }
}
