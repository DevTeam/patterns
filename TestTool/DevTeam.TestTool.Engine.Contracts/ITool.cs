namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public interface ITool
    {
        ToolType ToolType { get; }

        IDisposable Activate();
    }
}