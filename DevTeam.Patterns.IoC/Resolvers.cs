namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

		public static IEnumerable<T> ResolveAll<T>(this IResolver resolver)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			
			return
				resolver.Resolve(key => key.InstanceType == typeof(T) && key.StateType == typeof(EmptyState), key => EmptyState.Shared)
				.Select(i => i.Item2)
				.Cast<T>();
		}

		public static IEnumerable<T> ResolveAll<TState, T>(this IResolver resolver, Func<string, TState> stateSelector)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (stateSelector == null) throw new ArgumentNullException(nameof(stateSelector));

			return
				resolver.Resolve(key => key.InstanceType == typeof(T) && key.StateType == typeof(TState), key => stateSelector(key.Name))
				.Select(i => i.Item2)
				.Cast<T>();
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