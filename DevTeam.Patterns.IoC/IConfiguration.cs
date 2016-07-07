namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    public interface IConfiguration
    {
        IEnumerable<IConfiguration> GetDependencies();

        IEnumerable<IDisposable> CreateRegistrations(IContainer container);
	}
}