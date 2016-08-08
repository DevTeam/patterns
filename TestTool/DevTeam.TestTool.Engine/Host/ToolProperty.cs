namespace DevTeam.TestTool.Engine.Host
{
    using Contracts;

    internal class ToolProperty: BaseProperty
    {
        public ToolProperty()
            : base("tool", "Tool Name", "(-tool)", $"({WellknownTool.Explorer}|{WellknownTool.Runner}|{WellknownTool.Reporter}|{WellknownTool.Publisher})")
        {
        }
    }
}
