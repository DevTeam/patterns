namespace DevTeam.TestTool.Engine.Explorer
{
    using System;

    using Patterns.Reactive;
    using Contracts;

    internal class TestsSource : ITestsSource
    {
        public IObservable<Test> Create()
        {
            return Observable.Create<Test>(
                observer =>
                    {
                        return null;
                    });
        }
    }
}
