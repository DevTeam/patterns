namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;

    public interface IRegistrationComparer: IEqualityComparer<IRegistration>, IContainerContext
    {
    }
}
