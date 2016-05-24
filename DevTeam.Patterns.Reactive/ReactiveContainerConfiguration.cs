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

            container = container.Resolve<IContainer>(typeof(ReactiveContainerConfiguration).Name);

            var taskFactory = new TaskFactory();
            var sharedSingleThreadScheduler = new Lazy<IScheduler>(() => CreateSingleThreadScheduler(taskFactory));
            var sharedMultiThreadScheduler = new Lazy<IScheduler>(() => CreateMultiThreadScheduler(taskFactory));

            container
                .Register(() => sharedSingleThreadScheduler.Value, WellknownSchedulers.SharedSingleThread)
                .Register(() => CreateSingleThreadScheduler(taskFactory), WellknownSchedulers.PrivateSingleThread)
                .Register(() => sharedMultiThreadScheduler.Value, WellknownSchedulers.SharedMultiThread)
                .Register(() => CreateMultiThreadScheduler(taskFactory), WellknownSchedulers.PrivateMultiThread);

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
