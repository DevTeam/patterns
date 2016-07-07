namespace DevTeam.Patterns.EventAggregator
{
    using System;
    using System.Collections.Generic;

    using IoC;

    /// <inheritdoc/>
    public class EventAggregatorContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<IEventAggregator>(() => new Aggregator(container));            
        }        
    }
}