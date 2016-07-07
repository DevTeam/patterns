namespace Echo
{
    using System;

    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.IoC;

    public class Program
    {
        public static void Main()
        {
            // Create root IoC container
            using (var container = new Container())
            // Apply configuration
            using (new EchoConfiguration().Apply(container))
            // Activate EchoService 1
            using (container.Resolve<string, IEchoService>("1").Activate())
            // Activate EchoService 2
            using (container.Resolve<string, IEchoService>("2").Activate())
            // Register Console Echo publisher in EventAggregator as a consumer
            using (container.Resolve<IEventAggregator>().RegisterConsumer(container.Resolve<IEchoPublisher>()))
            // Register Console EchoRequest source in EventAggregator as a provider
            using (container.Resolve<IEventAggregator>().RegisterProvider(container.Resolve<IEchoRequestSource>()))
            {
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }       
    }
}
