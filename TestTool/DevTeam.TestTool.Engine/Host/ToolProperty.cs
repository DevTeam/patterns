namespace DevTeam.TestTool.Engine.Host
{
    using Contracts;

    internal class ToolProperty: BaseProperty
    {
        public static readonly IProperty Shared = new ToolProperty();

        private ToolProperty()
            : base("tool", "Tool Name", "(-tool)", $"({WellknownTool.Explorer}|{WellknownTool.Runnner}|{WellknownTool.Reporter})")
        {
        }
    }
}
