namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Threading.Tasks;

    using IoC;

    public class ReactiveContainerConfiguration: IConfiguration
    {
        public IContainer Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container = container.Resolve<IContainer>(nameof(ReactiveContainerConfiguration));
            container = new IoCContainerConfiguration().Apply(container);

            var taskFactory = new TaskFactory();

            container
                .Register(() => CreateSingleThreadScheduler(taskFactory), WellknownScheduler.PrivateSingleThread);

            container
                .Register(() => CreateMultiThreadScheduler(taskFactory), WellknownScheduler.PrivateMultiThread);

            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register(() => CreateSingleThreadScheduler(taskFactory), WellknownScheduler.SharedSingleThread);

            container
                .Using<ILifetime>(WellknownLifetime.Singletone)
                .Register(() => CreateMultiThreadScheduler(taskFactory), WellknownScheduler.SharedMultiThread);

            return container;
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
