namespace DevTeam.TestTool.Engine.Host
{
    internal class ToolProperty: BaseProperty
    {
        public ToolProperty()
            : base("tool", "Tool Name", "(-tool)", $"({WellknownTool.Explorer}|{WellknownTool.Runnner}|{WellknownTool.Reporter}|{WellknownTool.Publisher})")
        {
        }
    }
}
