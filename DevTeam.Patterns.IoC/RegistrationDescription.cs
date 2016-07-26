namespace DevTeam.Patterns.IoC
{
    using System;

    internal struct RegistrationDescription
    {
        public RegistrationDescription(Type stateType, Type contractType, object key, IDisposable resources)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (resources == null) throw new ArgumentNullException(nameof(resources));

            StateType = stateType;
            ContractType = contractType;
            Key = key;
            Resources = resources;
        }

        public Type StateType { get; }

        public Type ContractType { get; }

        public object Key { get; }

        public IDisposable Resources { get; }
    }
}
