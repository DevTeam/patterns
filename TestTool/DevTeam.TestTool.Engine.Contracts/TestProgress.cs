namespace DevTeam.TestTool.Engine.Contracts
{
    public class TestProgress
    {
        public TestProgress(TestState testState)
        {
            TestState = testState;
        }

        public TestProgress(TestState testState, TestResult result)
        {
            TestState = testState;
            Result = result;
        }

        public TestState TestState { get; private set; }

        public TestResult Result { get; private set; }
    }
}
