namespace Echo
{
    using System;

    using DevTeam.Patterns.IoC;
    using DevTeam.Platform.System;

    internal class ConsoleEchoPublisher: IEchoPublisher
    {
        private readonly IConsole _console;

        public ConsoleEchoPublisher(
            IConsole console)
        {
            if (console == null) throw new ArgumentNullException(nameof(console));

            _console = console;
        }

        public void OnNext(IEcho value)
        {
            _console.WriteLine(value.EchoMessage);
        }

        public void OnError(Exception error)
        {            
        }

        public void OnCompleted()
        {
            _console.WriteLine("Done.");
        }
    }
}
