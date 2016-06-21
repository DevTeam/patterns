namespace DevTeam.TestTool.dotNet
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
