namespace DevTeam.TestTool.Engine.Host
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Contracts;

    internal class Session : ISession
    {
        public Session(IEnumerable<PropertyValue> properties)
        {
            Properties = new ReadOnlyCollection<PropertyValue>(new List<PropertyValue>(properties));
        }

        public IEnumerable<PropertyValue> Properties { get; private set; }
    }
}
