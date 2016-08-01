namespace DevTeam.Patterns.IoC
{
    using System;

    internal class InternalScope : IScope
    {
        private readonly IResolver _resolver;

        public InternalScope(IResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            _resolver = resolver;
        }

        public bool Satisfy(IResolver resolver)
        {
            return Equals(_resolver, resolver);
        }
    }
}