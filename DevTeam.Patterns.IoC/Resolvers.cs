namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Threading.Tasks;

    public static class Resolvers
    {
        public static T Resolve<T>(this IResolver resolver, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var service = resolver.Resolve<EmptyState, T>(EmptyState.Shared, name);
            return service;
        }

        public static async Task<T> ResolveAsync<T>(this IResolver resolver, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return await resolver.Resolve<Task<T>>(name);
        }

        public static async Task<T> ResolveAsync<TState, T>(this IResolver resolver, TState arg, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return await resolver.Resolve<TState, Task<T>>(arg, name);
        }
    }
}