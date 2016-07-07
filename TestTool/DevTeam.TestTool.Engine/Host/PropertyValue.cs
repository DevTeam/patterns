namespace DevTeam.TestTool.Engine.Host
{
    using Contracts;

    internal class PropertyValue : PropertyValueDescription, IPropertyValue
    {
        public PropertyValue(PropertyValueDescription description)
            :base(description.Property, description.Value)
        {            
        }        
    }
}
