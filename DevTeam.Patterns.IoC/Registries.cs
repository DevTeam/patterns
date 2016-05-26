namespace DevTeam.Patterns.IoC
{
    using System;

    public static class Registries
    {
        public static IRegistry Register<T>(this IRegistry registry, Func<T> factory, string name = "")
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return registry.Register(new Func<EmptyState, T>(ignoredArg => factory()), name);
        }
    }
}
