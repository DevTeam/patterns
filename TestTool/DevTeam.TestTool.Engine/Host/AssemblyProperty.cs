namespace DevTeam.TestTool.Engine.Host
{
    using Contracts;

    internal class AssemblyProperty: BaseProperty
    {
        public static readonly IProperty Shared = new AssemblyProperty();

        private AssemblyProperty()
            : base("assembly", "Testing assembly", "assembly", ".+")
        {
        }
    }
}
