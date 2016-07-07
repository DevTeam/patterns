namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using IoC;

    /// <inheritdoc/>
    public class ReactiveContainerConfiguration: IConfiguration
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

            var taskFactory = new TaskFactory();

            yield return container.Register(() => CreateSingleThreadScheduler(taskFactory), WellknownScheduler.PrivateSingleThread);
            yield return container.Register(() => CreateMultiThreadScheduler(taskFactory), WellknownScheduler.PrivateMultiThread);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register(() => CreateSingleThreadScheduler(taskFactory), WellknownScheduler.SharedSingleThread);
            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register(() => CreateMultiThreadScheduler(taskFactory), WellknownScheduler.SharedMultiThread);
        }        

        private static Scheduler CreateMultiThreadScheduler(TaskFactory taskFactory)
        {
            if (taskFactory == null) throw new ArgumentNullException(nameof(taskFactory));

            return new Scheduler(taskFactory, Environment.ProcessorCount);
        }

        private static IScheduler CreateSingleThreadScheduler(TaskFactory taskFactory)
        {
            if (taskFactory == null) throw new ArgumentNullException(nameof(taskFactory));

            return new Scheduler(taskFactory, 1);
        }
    }
}
