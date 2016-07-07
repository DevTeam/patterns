namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class Resolvers
    {        
        public static T Resolve<TState, T>(this IResolver resolver, TState state, IComparable name = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return (T)resolver.Resolve(typeof(TState), typeof(T), state, name);
        }

        public static T Resolve<T>(this IResolver resolver, IComparable name = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));            

            return (T)resolver.Resolve(typeof(EmptyState), typeof(T), EmptyState.Shared, name);            
        }

		public static IEnumerable<T> ResolveAll<T>(this IResolver resolver)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            
		    return 
                from key in resolver.Registrations
		        where key.InstanceType == typeof(T) && key.StateType == typeof(EmptyState)
		        select (T)resolver.Resolve(key.StateType, key.InstanceType, EmptyState.Shared, key.Name);
		}

		public static IEnumerable<T> ResolveAll<TState, T>(this IResolver resolver, Func<IComparable, TState> stateSelector)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (stateSelector == null) throw new ArgumentNullException(nameof(stateSelector));

			return                
                from registration in resolver.Registrations
                where registration.InstanceType == typeof(T) && registration.StateType == typeof(TState)
                select (T)resolver.Resolve(registration.StateType, registration.InstanceType, stateSelector(registration.Name), registration.Name);
		}

		public static async Task<T> ResolveAsync<T>(this IResolver resolver, IComparable name = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));            

            return await resolver.Resolve<Task<T>>(name);
        }

        public static async Task<T> ResolveAsync<TState, T>(this IResolver resolver, TState state, IComparable name = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));            

            return await resolver.Resolve<TState, Task<T>>(state, name);
        }

        public static IContainer CreateChildContainer(this IResolver resolver, IComparable name = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return resolver.Resolve<IContainer>(name);
        }

        public static IResolver<T> Resolver<T>(this IResolver resolver, IComparable name = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return resolver.Resolve<IResolver<T>>(name);
        }

        public static IResolver<TState, T> Resolver<TState, T>(this IResolver resolver, IComparable name = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return resolver.Resolve<IResolver<TState, T>>(name);
        }
    }
}