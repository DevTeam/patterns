namespace DevTeam.TestTool.Engine.Host
{
    using System;

    using Contracts;

    internal class InstanceFactory : IInstanceFactory
    {
        public object Create(Type testFixtureType)
        {
            if (testFixtureType == null) throw new ArgumentNullException(nameof(testFixtureType));

            return Activator.CreateInstance(testFixtureType);
        }
    }
}
