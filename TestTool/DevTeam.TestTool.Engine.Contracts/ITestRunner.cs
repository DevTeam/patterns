namespace DevTeam.TestTool.Engine.Contracts
{
    public interface ITestRunner
    {
        TestResult Run(Test test);
    }
}