namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class TestResult
    {
        public TestResult(object result)
        {
            Result = result;
        }

        public TestResult(Exception exception)
        {
            Exception = exception;
        }

        public object Result { get; private set; }

        public Exception Exception { get; private set; }
    }
}
