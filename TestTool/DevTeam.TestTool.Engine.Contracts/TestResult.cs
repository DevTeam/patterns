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

        public TestResult(Test test, Exception exception)
        {
            if (test == null) throw new ArgumentNullException(nameof(test));

            Test = test;
            Exception = exception;
        }

        public Test Test { get; private set; }

        public object Result { get; private set; }

        public Exception Exception { get; private set; }
    }
}
