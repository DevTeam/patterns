namespace DevTeam.Patterns.IoC
{
    using System;

    internal interface IRegistry
	{
		IRegistry Register<T>(Func<T> factory, string name = "");

		IRegistry Register<TArg, T>(Func<TArg, T> factory, string name = "");
	}
}