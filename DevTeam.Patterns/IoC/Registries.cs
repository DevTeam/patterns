﻿namespace DevTeam.Patterns.IoC
{
    using System;

    public static class Registries
    {
        public static IRegistration Register<TState, T>(this IRegistry registry, Func<TState, T> factoryMethod, object key = null)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));            

            return registry.Register(typeof(TState), typeof(T), ctx => factoryMethod((TState)ctx.State), key);
        }

        public static IRegistration Register<T>(this IRegistry registry, Func<T> factoryMethod, object key = null)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            return registry.Register(new Func<EmptyState, T>(ignoredArg => factoryMethod()), key);
        }

        public static IContainer Using<TContext>(this IContainer container, object contextKey = null)
            where TContext: IContext
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return container.Using(() => container.Resolve<TContext>(contextKey));
        }

        public static IContainer Using<TContext>(this IContainer container, Func<TContext> contextFactory)
            where TContext : IContext
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (contextFactory == null) throw new ArgumentNullException(nameof(contextFactory));

            return (IContextContainer<TContext>)container.Resolve(typeof(ContextContainerState), typeof(IContextContainer<TContext>), new ContextContainerState(container, () => contextFactory() as IContext));
        }
    }    
}