namespace DevTeam.Patterns.IoC
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

            return container.Using(container.Resolve<TContext>(contextKey), typeof(TContext));
        }

        public static IContainer Using(this IContainer container, Type contextType, object contextKey = null)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (contextType == null) throw new ArgumentNullException(nameof(contextType));

            var context = (IContext)container.Resolve(typeof(EmptyState), contextType, contextKey);
            return (IContainer)container.Resolve(container, typeof(ContextContainerState), typeof(IContextContainer), new ContextContainerState(container, context, contextType));
        }

        public static IContainer Using(this IContainer container, IContext context, Type contextType)            
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (contextType == null) throw new ArgumentNullException(nameof(contextType));

            return (IContainer)container.Resolve(container, typeof(ContextContainerState), typeof(IContextContainer), new ContextContainerState(container, context, contextType));
        }
    }    
}