namespace IoC.Contracts
{
    using System.Collections.Generic;

    public interface IRegistration
    {
        IContainer Container { get; }

        IKey Key { get; }

        IEnumerable<IExtension> Extensions { get; }
    }
}
