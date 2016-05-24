namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public interface ITool
    {
        IDisposable Run();
    }
}