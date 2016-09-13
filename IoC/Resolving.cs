namespace IoC
{
    using System.Collections.Generic;

    using Contracts;

    internal class Resolving: IResolving
    {
        public Resolving(IContainer container, IRegistration registration, IEnumerable<object> states)
        {
            Container = container;
            Registration = registration;
            States = states;
        }

        public IContainer Container { get; }

        public IRegistration Registration { get; }

        public IEnumerable<object> States { get; }
    }
}
