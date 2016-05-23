namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class TestMethod
    {
        public TestMethod(TestFixture fixture, string name)
        {
            if (fixture == null) throw new ArgumentNullException(nameof(fixture));
            if (name == null) throw new ArgumentNullException(nameof(name));

            Fixture = fixture;
            Name = name;
        }

        public TestFixture Fixture { get; private set; }

        public string Name { get; private set; }
    }
}
