namespace DevTeam.Patterns.EventAggregator
{
    using System;

    using Dispose;
    using IoC;

    public class EventAggregatorContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();

            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEventAggregator>(() => new Aggregator()));            

            return disposable;
        }
    }
}