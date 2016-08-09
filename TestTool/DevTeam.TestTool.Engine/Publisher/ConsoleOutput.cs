namespace DevTeam.TestTool.Engine.Publisher
{
    using System;

    using Contracts;

    using Platform.System;

    internal class ConsoleOutput : IOutput
    {
        private readonly IConsole _console;

        public ConsoleOutput(
            IConsole console)
        {
            if (console == null) throw new ArgumentNullException(nameof(console));

            _console = console;
        }

        public void WriteReport(string report)
        {
            _console.WriteLine(report);
        }
    }
}
