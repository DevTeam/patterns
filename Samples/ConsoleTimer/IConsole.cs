namespace ConsoleTimer
{
    internal interface IConsole
    {
        void WriteLine(string line);

        string ReadLine();
    }
}