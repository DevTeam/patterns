namespace DevTeam.TestTool.Engine.Host
{
    internal class AssemblyProperty: BaseProperty
    {
        public AssemblyProperty()
            : base("assembly", "Testing assembly", "assembly", ".+")
        {
        }
    }
}
