namespace DevTeam.TestTool.Engine.Contracts
{
    using System;

    public interface IInstanceFactory
    {
        object Create(Type testFixtureType);
    }
}