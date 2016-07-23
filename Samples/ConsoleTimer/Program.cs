namespace ConsoleTimer
{
    using System;

    using DevTeam.Patterns.IoC;
    using DevTeam.Platform.System;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        public static void Main()
        {
            // Create root IoC container
            using (var container = new Container())
            // Apply configuration
            using (new ConsoleTimerConfiguration().Apply(container))
            {
                // Create console
                var console = container.Resolve<IConsole>();
                console.WriteLine("Press Enter to exit");

                // Create publisher
                using (container.Resolve<TimeSpan, ITimePublisher>(TimeSpan.FromSeconds(1)))
                {
                    // Wait for any key
                    console.ReadLine();                    
                }                
            }
        }
    }
}
