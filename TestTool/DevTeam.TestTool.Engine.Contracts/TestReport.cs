namespace DevTeam.TestTool.Engine.Contracts
{
    public class TestReport
    {
        public TestReport(Test test, string report)
        {
            Test = test;
            Report = report;
        }

        public Test Test { get; private set; }

        public string Report { get; private set; }
    }
}
