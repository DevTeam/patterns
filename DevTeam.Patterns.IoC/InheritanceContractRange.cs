namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal class InheritanceContractRange : IContractRange
    {
        private readonly IContractRange _baseRange;
        
        public InheritanceContractRange(IContractRange baseRange)
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

            foreach (var baseType in GetBaseTypes(registration.ContractType))
            {
                yield return new Registration(registration.StateType, baseType, registration.Key);
            }
        }

        private static IEnumerable<Type> GetBaseTypes(Type type)
        {
            do
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
            while (type != null && type != typeof(object));
        }
    }
}
