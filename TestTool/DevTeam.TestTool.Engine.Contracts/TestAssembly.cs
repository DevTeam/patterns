namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class TestAssembly
    {
        public TestAssembly(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
