namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    public interface IConfiguration
    {
        IEnumerable<IConfiguration> GetDependencies();

        IEnumerable<IRegistration> CreateRegistrations(IContainer container);
	}
}