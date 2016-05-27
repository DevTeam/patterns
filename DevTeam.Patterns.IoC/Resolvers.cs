namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Threading.Tasks;

    public static class Resolvers
    {        
        public static T Resolve<TState, T>(this IResolver resolver, TState state, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return (T)resolver.Resolve(typeof(TState), typeof(T), state, name);
        }

        public static T Resolve<T>(this IResolver resolver, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return (T)resolver.Resolve(typeof(EmptyState), typeof(T), EmptyState.Shared, name);            
        }

        public static async Task<T> ResolveAsync<T>(this IResolver resolver, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return await resolver.Resolve<Task<T>>(name);
        }

        public static async Task<T> ResolveAsync<TState, T>(this IResolver resolver, TState state, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return await resolver.Resolve<TState, Task<T>>(state, name);
        }
    }
}