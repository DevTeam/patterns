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

        public IEnumerable<IRegistration> Registrations => _state.TargetContainer.Registrations;

        public IRegistration Register(Type stateType, Type contractType, Func<IResolvingContext, object> factoryMethod, object key = null)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            using (_state.TargetContainer.Register(() => (TContext)_state.Context))
            {
                return _state.TargetContainer.Register(stateType, contractType, factoryMethod, key);
            }
        }

        public object Resolve(IResolver resolver, Type stateType, Type contractType, object state, object key = null)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (state == null) throw new ArgumentNullException(nameof(state));

            using (_state.TargetContainer.Register(() => (TContext)_state.Context))
            {
                return _state.TargetContainer.Resolve(resolver, stateType, contractType, state, key);
            }
        }

        public void Dispose()
        {
            _state.TargetContainer.Dispose();
        }
    }
}