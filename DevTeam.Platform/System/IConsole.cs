namespace DevTeam.Platform.System
{
    public interface IConsole
    {
        void WriteLine(string line);

        string ReadLine();
    }
}