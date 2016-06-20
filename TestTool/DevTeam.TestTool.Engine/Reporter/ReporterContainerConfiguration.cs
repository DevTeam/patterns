namespace DevTeam.TestTool.Engine.Reporter
{
    using System;

    using Patterns.Dispose;
    using Patterns.EventAggregator;
    using Patterns.IoC;
    using Contracts;

    public class ReporterContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(container
                .Register<ISession, ITool>(session => new ReporterTool(
                    session,
                    container.ResolveAll<ITestReporter>(),
                    container.Resolve<IEventAggregator>())));

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<ITestReporter>(() => new TextTestReporter(), "text"));

            return disposable;
        }
    }
}
