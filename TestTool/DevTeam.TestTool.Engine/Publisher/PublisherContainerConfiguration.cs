namespace DevTeam.TestTool.Engine.Publisher
{
    using System;

    using Patterns.Dispose;
    using Patterns.EventAggregator;
    using Patterns.IoC;
    using Contracts;
    using Host;    

    public class PublisherContainerConfiguration : IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(container
                .Register<ISession, ITool>(session => new PublisherTool(
                    session,
                    container.ResolveAll<IReportPublisher>(),
                    container.Resolve<IEventAggregator>())));

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IReportPublisher>(() => new ReportPublisher(container.ResolveAll<IOutput>())));

            return disposable;
        }
    }
}
