namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class ContextContainer<TContext>: IContextContainer
        where TContext: IContext
    {
        private readonly ContextContainerState _state;
        
        public ContextContainer(ContextContainerState state)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));

            _state = state;
        }

        public object Key => _state.TargetContainer.Key;

        public IEnumerable<IRegistration> GetRegistrations()
        {
            return _state.TargetContainer.GetRegistrations();
        }

        public IEnumerable<IRegistration> GetRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return _state.TargetContainer.GetRegistrations(container);
        }

        public IRegistration Register(
            Type stateType,
            Type contractType,
            Func<IResolvingContext, object> factoryMethod,
            object key = null)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            using (_state.TargetContainer.Register(typeof(EmptyState), typeof(TContext), ctx => (TContext)_state.Context))
            {
                return _state.TargetContainer.Register(stateType, contractType, factoryMethod, key);
            }
        }

        public IRegistration Register(
            IContainer registerContainer,
            IRegistration registration,
            Func<IResolvingContext, object> factoryMethod)
        {
            if (registerContainer == null) throw new ArgumentNullException(nameof(registerContainer));
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            using (_state.TargetContainer.Register(typeof(EmptyState), typeof(TContext), ctx => (TContext)_state.Context))
            {
                return _state.TargetContainer.Register(registerContainer, registration, factoryMethod);
            }
        }

        public object Resolve(
            Type stateType,
            Type contractType,
            object state,
            object key = null)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (state == null) throw new ArgumentNullException(nameof(state));

            return _state.TargetContainer.Resolve(stateType, contractType, state, key);
        }

        public object Resolve(IContainer resolveContainer, IRegistration registration, object state)
        {
            if (resolveContainer == null) throw new ArgumentNullException(nameof(resolveContainer));
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (state == null) throw new ArgumentNullException(nameof(state));

            return _state.TargetContainer.Resolve(resolveContainer, registration, state);
        }

        public void Dispose()
        {
            _state.TargetContainer.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(ContextContainer<TContext>)} [TargetContainer: {_state.TargetContainer}, ContextType: {_state?.ContextType?.Name ?? "null"}, Context: {_state.Context?.ToString() ?? "null"}]";
        }
    }
}