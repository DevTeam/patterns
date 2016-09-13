namespace IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Contracts;

    using DevTeam.Patterns.Dispose;

    public class Container: IContainer
    {
        private readonly Dictionary<IKey, Registration> _registrations = new Dictionary<IKey, Registration>();

        public object Key { get; }

        public IEnumerable<IRegistration> Registrations => _registrations.Values.Distinct();

        public bool TryRegister(out IDisposable registrationToken, IKey key, IFactory factory, params IExtension[] extensions)
        {
            var registration = extensions.OfType<Registration>().SingleOrDefault() ?? new Registration(this, key, extensions, factory);

            var token = new CompositeDisposable();
            registrationToken = token;

            var keys =
                from keyPart in key.Contracts
                select Keys.Create(key.Key).Implementing(keyPart).Receiving(key.States);

            foreach (var curKey in keys)
            {
                if (_registrations.ContainsKey(curKey))
                {
                    token.Clear();
                    return false;
                }

                _registrations.Add(curKey, registration);
                token.Add(Disposable.Create(() => Unregister(curKey)));
            }

            return true;
        }

        public bool TryResolve(out object value, IKey key, params object[] states)
        {
            Registration registration;
            if (!_registrations.TryGetValue(key, out registration))
            {
                value = default(object);
                return false;
            }

            var resolving = new Resolving(this, registration, states);
            value = registration.Factory.Create(resolving);
            return true;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private void Unregister(IKey key)
        {
        }
    }
}
