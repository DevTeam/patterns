namespace DevTeam.Patterns.IoC
{
    using System;

    internal class Resolver<TState, T> : IResolver<TState, T>
    {
        private readonly IResolver _resolver;

        public Resolver(IResolver resolver)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            _resolver = resolver;
        }

        
        public T Resolve(TState state, string name = "")
        {
            return _resolver.Resolve<TState, T>(state, name);
        }
    }
}