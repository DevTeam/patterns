namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class Test
    {
        public Test(TestMethod method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            Id = Guid.NewGuid();
            Method = method;
        }

        public Guid Id { get; }

        public TestMethod Method { get; }

        public override string ToString()
        {
            return $"{Id} {Method}";
        }
    }
}
