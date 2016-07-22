using System;

namespace DevTeam.Platform.System
{
    public class Console: IConsole
    {
        public void WriteLine(string line)
        {
            if (line == null) throw new ArgumentNullException(nameof(line));

            global::System.Console.WriteLine(line);
        }

        public string ReadLine()
        {
            return global::System.Console.ReadLine();
        }
    }
}
