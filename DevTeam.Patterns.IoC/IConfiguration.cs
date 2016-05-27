namespace DevTeam.Patterns.IoC
{
    using System;
    public interface IConfiguration
	{
		IContainer Apply(IContainer container);
	}
}