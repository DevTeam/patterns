namespace DevTeam.TestTool.Engine.Host
{
    using Contracts;

    internal interface IPropertyFactory
    {
        IPropertyValue CreatePropertyValue(string propertyId, string propertyValue);
    }
}