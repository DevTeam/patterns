namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class TestResult
    {
        public TestResult(Test test, object result)
        {
            if (test == null) throw new ArgumentNullException(nameof(test));

            Test = test;
            Result = result;
        }

        public Test Test { get; private set; }

        public object Result { get; private set; }
    }
}
