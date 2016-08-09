namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public class SummariseReport
    {
        public SummariseReport(int testTotals, int testFails, int testSuccess)
        {
            if (testTotals < 0) throw new ArgumentOutOfRangeException(nameof(testTotals));
            if (testFails < 0 || testFails > testTotals) throw new ArgumentOutOfRangeException(nameof(testFails));
            if (testSuccess < 0 || testSuccess > testTotals) throw new ArgumentOutOfRangeException(nameof(testSuccess));

            TestTotals = testTotals;
            TestFails = testFails;
            TestSuccess = testSuccess;
        }

        public int TestTotals { get; }

        public int TestFails { get; }

        public int TestSuccess { get; }
    }
}
