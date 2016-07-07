namespace DevTeam.Patterns.EventAggregator
{
    using System;
    using System.Collections.Generic;

    using IoC;

    public class EventAggregatorContainerConfiguration: IConfiguration
    {
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IEventAggregator>(() => new Aggregator());            
        }        
    }
}