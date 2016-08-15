namespace HelloWorld
{
    using System;

    using DevTeam.Patterns.IoC;

    class Program
    {
        static void Main(string[] args)
        {
            // Step 1
            var container = Containers.Create();
            // Step 2
            container.Register<ConsoleService>().As<ConsoleService>();
            container.Register<HelloWorldWriter>().As<HelloWorldWriter>();
            // Step 3
            var hellowWorldWriter = container.Resolve<HelloWorldWriter>();
            hellowWorldWriter.Write();            
        }

        // Client
        class HelloWorldWriter
        {
            private readonly ConsoleService _consoleService;

            public HelloWorldWriter(ConsoleService consoleService)
            {
                _consoleService = consoleService;
            }

            public void Write()
            {
                _consoleService.WriteLine("Hello world!");
            }
        }

        // Service
        class ConsoleService
        {
            public void WriteLine(string line)
            {            
                Console.WriteLine(line);    
            }
        }
    }
}
