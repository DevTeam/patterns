namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CustomContractRange: IContractRange
    {
        private readonly IContractRange _baseRange;
        private readonly IEnumerable<Type> _types;

        public CustomContractRange(IContractRange baseRange, IEnumerable<Type> types)
        {
            _baseRange = baseRange;
            _types = types;
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

            foreach (var type in _types)
            {
                yield return new Registration(registration.StateType, type, registration.Key);
            }
        }
    }
}
