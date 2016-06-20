namespace DevTeam.TestTool.Engine.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface ISession: IDisposable
    {
        IEnumerable<PropertyValue> Properties { get; }
    }
}