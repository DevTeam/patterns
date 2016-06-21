namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class TestFixture
    {
        public TestFixture(TestAssembly assembly, string name)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (name == null) throw new ArgumentNullException(nameof(name));

            Assembly = assembly;
            Name = name;
        }

        public TestAssembly Assembly { get; }

        public string Name { get; }

        public override string ToString()
        {
            return $"{Assembly}:{Name}";
        }
    }
}
