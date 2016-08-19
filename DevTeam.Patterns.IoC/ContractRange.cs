namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    internal class ContractRange: IContractRange
    {
        public IEnumerable<IRegistration> GetRegisterVariants(IRegistration registration)
        {
            yield return registration;
        }
    }
}
