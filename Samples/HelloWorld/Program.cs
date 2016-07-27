namespace HelloWorld
{
    using DevTeam.Patterns.IoC;

    public class Program
    {
        public static void Main(string[] args)
        {
            var container = new Container();            
            container.Register(() => new ConsoleService());
            container.Register(() => new HelloWorldWriter(container.Resolve<ConsoleService>()));

            var hellowWorldWriter = container.Resolve<HelloWorldWriter>();
            hellowWorldWriter.Write();            
        }

        // Client
        private class HelloWorldWriter
        {
            private readonly ConsoleService _consoleService;

            public HelloWorldWriter(
                // Dependency injection
                ConsoleService consoleService)
            {
                _consoleService = consoleService;
            }

            public void Write()
            {
                _consoleService.WriteLine("Hello world!");
            }
        }

        // Service
        private class ConsoleService
        {
            public void WriteLine(string line)
            {            
                System.Console.WriteLine(line);    
            }
        }
    }
}
