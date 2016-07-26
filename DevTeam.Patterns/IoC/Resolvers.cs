namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class Resolvers
    {        
        public static T Resolve<TState, T>(this IResolver resolver, TState state, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return (T)resolver.Resolve(resolver, typeof(TState), typeof(T), state, key);
        }

        public static T Resolve<T>(this IResolver resolver, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));            

            return (T)resolver.Resolve(resolver, typeof(EmptyState), typeof(T), EmptyState.Shared, key);            
        }

        public static object Resolve(this IResolver resolver, Type stateType, Type contractType, object state, object key = null)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            return resolver.Resolve(null, stateType, contractType, state, key);
        }

        public static IEnumerable<T> ResolveAll<T>(this IResolver resolver)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            return resolver.Resolve<IEnumerable<T>>();
        }

		public static IEnumerable<T> ResolveAll<TState, T>(this IResolver resolver, Func<object, TState> stateSelector)
		{
			if (resolver == null) throw new ArgumentNullException(nameof(resolver));
			if (stateSelector == null) throw new ArgumentNullException(nameof(stateSelector));

            return resolver.Resolve<StateSelector, IEnumerable<T>>(ctx => stateSelector(ctx.Registration.Key) as object);
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