namespace ConsoleTimer
{
    using DevTeam.Patterns.IoC;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        public static void Main()
        {
            // Create root IoC container
            using (var container = new Container())
            // Apply configuration
            using (new TimerConfiguration().Apply(container))
            // Create publisher
            using (container.Resolve<ITimePublisher>())
            {
                // Wait for any key
                container.Resolve<IConsole>().ReadLine();
            }
        }
    }
}
