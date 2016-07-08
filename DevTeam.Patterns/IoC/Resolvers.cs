namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class Resolvers
    {        
        public static T Resolve<TState, T>(this IResolver resolver, TState state, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return (T)resolver.Resolve(typeof(TState), typeof(T), state, key);
        }

        public static T Resolve<T>(this IResolver resolver, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));            

            return (T)resolver.Resolve(typeof(EmptyState), typeof(T), EmptyState.Shared, key);            
        }

		public static IEnumerable<T> ResolveAll<T>(this IResolver resolver)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            
		    return 
                from key in resolver.Registrations
		        where key.InstanceType == typeof(T) && key.StateType == typeof(EmptyState)
		        select (T)resolver.Resolve(key.StateType, key.InstanceType, EmptyState.Shared, key.Key);
		}

		public static IEnumerable<T> ResolveAll<TState, T>(this IResolver resolver, Func<object, TState> stateSelector)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (stateSelector == null) throw new ArgumentNullException(nameof(stateSelector));

			return                
                from registration in resolver.Registrations
                where registration.InstanceType == typeof(T) && registration.StateType == typeof(TState)
                select (T)resolver.Resolve(registration.StateType, registration.InstanceType, stateSelector(registration.Key), registration.Key);
		}

		public static async Task<T> ResolveAsync<T>(this IResolver resolver, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));            

            return await resolver.Resolve<Task<T>>(key);
        }

        public static async Task<T> ResolveAsync<TState, T>(this IResolver resolver, TState state, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));            

            return await resolver.Resolve<TState, Task<T>>(state, key);
        }

        /// <summary>
        /// Creates a new nested Container as a child of the current container. The current container first applies its own settings, and then it checks the parent for additional settings. Returns a reference to the new container.
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IContainer CreateChildContainer(this IResolver resolver, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return resolver.Resolve<IContainer>(key);
        }

        public static IResolver<T> Resolver<T>(this IResolver resolver, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return resolver.Resolve<IResolver<T>>(key);
        }

        public static IResolver<TState, T> Resolver<TState, T>(this IResolver resolver, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return resolver.Resolve<IResolver<TState, T>>(key);
        }
    }
}