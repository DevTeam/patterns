﻿namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

	public static class Registries
    {
        public static IRegistration Register<TState, T>(this IRegistry registry, Func<TState, T> factoryMethod, IComparable name = null)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));            

            return registry.Register(typeof(TState), typeof(T), ctx => factoryMethod((TState)ctx.State), name);
        }

        public static IRegistration Register<T>(this IRegistry registry, Func<T> factoryMethod, IComparable name = null)
        {
            if (registry == null) throw new ArgumentNullException(nameof(registry));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            return registry.Register(new Func<EmptyState, T>(ignoredArg => factoryMethod()), name);
        }

        public static IContainer Using<TContext>(this IContainer container, string contextName)
            where TContext: IContainerContext
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (contextName == null) throw new ArgumentNullException(nameof(contextName));

            return container.Using(() => container.Resolve<TContext>(contextName));
        }

        public static IContainer Using<TContext>(this IContainer container, Func<TContext> factoryMethod)
            where TContext : IContainerContext
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            return new RegisterContainer<TContext>(container, factoryMethod);           
        }

        private class RegisterContainer<TContext> : IContainer
            where TContext: IContainerContext
        {
            private readonly IContainer _container;
            private readonly Func<TContext> _factoryMethod;

            public RegisterContainer(IContainer container, Func<TContext> factoryMethod)
            {
                if (container == null) throw new ArgumentNullException(nameof(container));
                if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

                _container = container;
                _factoryMethod = factoryMethod;
            }

            public IComparable Name => _container.Name;

            public IEnumerable<IRegistration> Registrations => _container.Registrations;

            public IRegistration Register(Type stateType, Type instanceType, Func<IResolvingContext, object> factoryMethod, IComparable name = null)
            {
	            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
	            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
	            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

	            using (_container.Register(_factoryMethod))
                {
                    return _container.Register(stateType, instanceType, factoryMethod, name);
                }
            }

            public object Resolve(Type stateType, Type instanceType, object state, IComparable name = null)
            {
	            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
	            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
	            if (state == null) throw new ArgumentNullException(nameof(state));

                using (_container.Register(_factoryMethod))
                {
                    return _container.Resolve(stateType, instanceType, state, name);
                }
            }

	        public void Dispose()
            {
                _container.Dispose();
            }
        }
    }    
}