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
            using (container.Apply(ConsoleTimerConfiguration.Shared))
            {
                // Create console
                var console = container.Resolve<IConsole>();

                // Create publisher
                using (container.Resolve<ITimePublisher>())
                {
                    // Wait for any key
                    console.ReadLine();                    
                }

                console.WriteLine("Press any key to exit");
                console.ReadLine();
            }
        }
    }
}
