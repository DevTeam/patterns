namespace DevTeam.Patterns.IoC
{
    using System;

    internal class Resolver<T> : IResolver<T>
    {
        private readonly IResolver _resolver;

        public Resolver(IResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            _resolver = resolver;
        }

        public T Resolve(IComparable name = null)
        {
            return _resolver.Resolve<T>(name);
        }
    }
}