namespace DevTeam.TestTool.Engine.Explorer
{
    using System;
    using System.Linq;

    using Contracts;

    internal class ExplorerTool : ITool
    {
        private readonly ISession _session;

        public ExplorerTool(ISession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            _session = session;
        }        

        public void Run()
        {
            var assemblies = _session.Properties.Where(p => Equals(p.Property, AssemblyProperty.Shared)).Select(p => p.Value);                       
        }
    }
}