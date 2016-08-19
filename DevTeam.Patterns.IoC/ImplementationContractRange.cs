namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class ImplementationContractRange : IContractRange
    {
        private readonly IContractRange _baseRange;

        public ImplementationContractRange(IContractRange baseRange)
        {
            _baseRange = baseRange;
        }

        public IEnumerable<IRegistration> GetRegisterVariants(IRegistration registration)
        {
            return GetRegisterVariantsInternal(registration).Distinct();
        }

        private IEnumerable<IRegistration> GetRegisterVariantsInternal(IRegistration registration)
        {
            foreach (var registrationVariant in _baseRange.GetRegisterVariants(registration))
            {
                yield return registrationVariant;
            }

            foreach (var implementedInterface in registration.ContractType.GetTypeInfo().ImplementedInterfaces)
            {
                yield return new Registration(registration.StateType, implementedInterface, registration.Key);
            }
        }
    }
}
