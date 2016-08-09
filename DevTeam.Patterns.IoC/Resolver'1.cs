namespace DevTeam.Patterns.IoC
{
    using System;

    internal class Resolver<T> : IResolver<T>
    {
        private readonly IResolver _resolver;
        private readonly object _defaultKey;

        public Resolver(IResolver resolver, object defaultKey)
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));

            _resolver = resolver;
            _defaultKey = defaultKey;
        }

        public T Resolve(object key = null)
        {
            return _resolver.Resolve<T>(key ?? _defaultKey);
        }

        public override string ToString()
        {
            return $"{nameof(Resolver<T>)} [BaseResolver: {_resolver}, DefaultKey: {_defaultKey?.ToString() ?? "null"}]";
        }
    }
}