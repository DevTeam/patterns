namespace DevTeam.TestTool.Engine.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface ISession: IDisposable
    {
        IEnumerable<IPropertyValue> Properties { get; }
    }
}