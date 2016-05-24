namespace DevTeam.TestTool.Engine.Contracts
{
    public interface IToolFactory
    {
        ITool Create(ISession session, string toolName = "");
    }
}