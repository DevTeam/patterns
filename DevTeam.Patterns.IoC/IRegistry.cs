namespace DevTeam.Patterns.IoC
{
    using System;

    public interface IRegistry
	{
		IRegistry Register<TState, T>(Func<TState, T> factory, string name = "");
	}
}