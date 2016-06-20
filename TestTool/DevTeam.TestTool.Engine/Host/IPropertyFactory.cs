namespace DevTeam.TestTool.Engine.Host
{
    using Contracts;

    internal interface IPropertyFactory
    {
        PropertyValue CreatePropertyValue(string propertyId, string propertyValue);
    }
}