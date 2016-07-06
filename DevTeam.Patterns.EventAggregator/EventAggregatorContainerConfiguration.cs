namespace DevTeam.Patterns.EventAggregator
{
    using System;
    using System.Collections.Generic;

    using IoC;

    public class EventAggregatorContainerConfiguration: IConfiguration
    {
        public static readonly IConfiguration Shared = new EventAggregatorContainerConfiguration();

        private EventAggregatorContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        public IEnumerable<IDisposable> Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singletone).Register<IEventAggregator>(() => new Aggregator());            
        }
    }
}