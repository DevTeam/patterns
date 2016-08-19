namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Linq;

    internal class RegistrationFactory: IRegistrationFactory
    {
        public IRegistration Create(IRegistrationDescription description, Type stateType, Type contractType, object key = null)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            var container = description.Container;

            if (description.Lifetime != WellknownLifetime.Transient)
            {
                container = container.Using<ILifetime>(description.Lifetime);
            }

            if (description.Comparer != WellknownComparer.FullCompliance)
            {
                container = container.Using<IComparer>(description.Comparer);
            }

            if (description.Scope != WellknownScope.Public)
            {
                container = container.Using<IScope>(description.Scope);
            }

            IContractRange contractRange = null;
            if (description.ContractRange != WellknownContractRange.Contract)
            {
                contractRange = container.Resolve<IContractRange>(description.ContractRange);
            }

            if (description.AdditionalContracts.Any())
            {
                contractRange = new CustomContractRange(contractRange ?? description.Container.Resolve<IContractRange>(), description.AdditionalContracts);
            }

            if (contractRange != null)
            {
                container = container.Using(contractRange, typeof(IContractRange));
            }

            return container.Register(stateType, contractType, description.ImplementationType, key);
        }
    }
}
