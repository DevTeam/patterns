namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    public interface IContractRange: IContext
    {
        IEnumerable<IRegistration> GetRegisterVariants(IRegistration registration);
    }
}
