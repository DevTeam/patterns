namespace DevTeam.TestTool.Engine.Contracts
{
    using System.Collections.Generic;

    public interface ISession
    {
        IEnumerable<PropertyValue> Properties { get; }
    }
}