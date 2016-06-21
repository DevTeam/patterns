namespace DevTeam.TestTool.Engine.Contracts
{
    public interface IProperty
    {
        string Id { get; }

        string Description { get; }

        bool Match(string name);

        bool Validate(string value);
    }
}
