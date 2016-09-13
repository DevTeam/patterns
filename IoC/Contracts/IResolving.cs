namespace IoC.Contracts
{
    using System.Collections.Generic;

    public interface IResolving
    {
        IContainer Container { get; }

        IRegistration Registration { get; }

        IEnumerable<object> States { get; }
    }
}
