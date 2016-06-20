namespace DevTeam.Patterns.Reactive
{
    using System;
    using System.Threading.Tasks;

    using Dispose;

    using IoC;

    public class ReactiveContainerConfiguration: IConfiguration
    {
        public IDisposable Apply(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var disposable = new CompositeDisposable();
            var taskFactory = new TaskFactory();

            disposable.Add(container.Register(() => CreateSingleThreadScheduler(taskFactory), WellknownScheduler.PrivateSingleThread));
            disposable.Add(container.Register(() => CreateMultiThreadScheduler(taskFactory), WellknownScheduler.PrivateMultiThread));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => CreateSingleThreadScheduler(taskFactory), WellknownScheduler.SharedSingleThread));
            disposable.Add(container.Using<ILifetime>(WellknownLifetime.Singletone).Register(() => CreateMultiThreadScheduler(taskFactory), WellknownScheduler.SharedMultiThread));

            return disposable;
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
