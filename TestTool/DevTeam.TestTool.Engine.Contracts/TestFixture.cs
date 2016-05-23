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

        public TestAssembly Assembly { get; private set; }

        public string Name { get; private set; }        
    }
}
