namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IConfiguration
	{
		IDisposable Apply(IContainer container);
	}
}