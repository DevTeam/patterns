namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class Test
    {
        public Test(TestMethod method)
        {
            if (method == null) throw new ArgumentNullException(nameof(method));

            Method = method;
        }

        public TestMethod Method { get; private set; }
    }
}
