namespace DevTeam.TestTool.dotNetCore
{
    using Engine.Contracts;

    internal class Console: IOutput
    {
        public void Write(string text)
        {
            System.Console.WriteLine(text);
        }
    }
}
