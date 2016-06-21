namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class TestProgress
    {
        public TestProgress(Test test, TestState testState)
        {
            if (test == null) throw new ArgumentNullException(nameof(test));

            Test = test;
            TestState = testState;
        }

        public TestProgress(Test test, TestState testState, TestResult result)
        {
            if (test == null) throw new ArgumentNullException(nameof(test));
            if (result == null) throw new ArgumentNullException(nameof(result));

            Test = test;
            TestState = testState;
            Result = result;
        }

        public Test Test { get; private set; }

        public TestState TestState { get; private set; }

        public TestResult Result { get; private set; }
    }
}
