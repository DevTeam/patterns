namespace DevTeam.TestTool.Engine.Contracts
{
    public class TestResult
    {
        public TestResult(Test test, object result)
        {
            Test = test;
            Result = result;
        }

        public Test Test { get; private set; }

        public object Result { get; private set; }
    }
}
