namespace DevTeam.TestTool.Engine.Host
{
    using DevTeam.TestTool.Engine.Contracts;

    internal class ToolProperty: BaseProperty
    {
        public static readonly IProperty Shared = new ToolProperty();

        private ToolProperty()
            : base("tool", "ExplorerTool name", "(-tool)", "(^$|explorer|runner)")
        {
        }
    }
}
