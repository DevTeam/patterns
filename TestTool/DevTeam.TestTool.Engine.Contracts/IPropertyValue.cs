namespace DevTeam.TestTool.Engine.Contracts
{
    public interface IPropertyValue
    {
        IProperty Property { get; }

        string Value { get; }
    }
}