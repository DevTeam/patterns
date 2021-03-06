﻿namespace ConsoleTimer
{
    using System.IO;

    using DevTeam.Patterns.Dispose;
    using DevTeam.Patterns.IoC;
    using DevTeam.Patterns.IoC.Configuration;
    using DevTeam.Platform.System;

    // ReSharper disable once ClassNeverInstantiated.Global
    public class Program
    {
        public static void Main()
        {
            // Create root IoC container
            using (var container = Containers.Create())
            using (container.Apply(new ConfigurationsContainerConfiguration()).ToCompositeDisposable())
            // Apply configuration
            using (container.Apply(container.Resolve<string, IConfiguration>(File.ReadAllText("ConsoleTimerContainerConfiguration.json"), WellknownConfigurations.Json)).ToCompositeDisposable())
            {
                // Create console
                var console = container.Resolve<IConsole>();
                console.WriteLine("Press Enter to exit");

                // Create publisher
                using (container.Resolve<ITimePublisher>())
                {
                    // Wait for any key
                    console.ReadLine();
                }
            }
        }
    }
}
