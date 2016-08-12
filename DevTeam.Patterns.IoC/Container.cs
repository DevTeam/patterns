namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;
    using Dictionary = System.Collections.Generic.Dictionary<IRegistration, Container.RegistrationInfo>;

    public class Container : IContainer
    {
        private static readonly Dictionary<Type, object> DefaultInstances = new Dictionary<Type, object>
        {
            { typeof(ILifetime), RootContainerConfiguration.TransientLifetime.Value },
            { typeof(IRegistrationComparer), RootContainerConfiguration.FullComplianceRegistrationComparer.Value },
            { typeof(IBinder), RootContainerConfiguration.Binder.Value },
            { typeof(IFactory), RootContainerConfiguration.Factory.Value },
            { typeof(IScope), RootContainerConfiguration.PublicScope.Value }
        };

        private static readonly ComparerForRegistrationComparer ComparerForRegistrationComparer = new ComparerForRegistrationComparer();
        private readonly SortedDictionary<IRegistrationComparer, Dictionary> _factories = new SortedDictionary<IRegistrationComparer, Dictionary>(ComparerForRegistrationComparer);
        private readonly Dictionary<RegistrationDescription, RegistrationInfo> _chache = new Dictionary<RegistrationDescription, RegistrationInfo>();
        private readonly IContainer _parentContainer;
        private readonly IDisposable _disposable = Disposable.Empty();

        /// <summary>
        /// Creates a default container with key/name. Returns a reference to the new resolver.
        /// </summary>
        /// <param name="key">Container's key. For example a name.</param>
        public Container(object key = null)
        {
            Key = key;
            _disposable =
                RootContainerConfiguration.Shared.CreateRegistrations(this)
                .ToCompositeDisposable();
        }

        internal Container(ContainerDescription containerDescription)
        {
            if (containerDescription == null) throw new ArgumentNullException(nameof(containerDescription));

            Key = containerDescription.Key;
            _parentContainer = containerDescription.ParentContainer;
        }

        public object Key { get; }

        private bool IsRoot => _parentContainer == null;

        public IEnumerable<IRegistration> GetRegistrations()
        {
            return GetRegistrations(this);
        }

        public IEnumerable<IRegistration> GetRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return 
                _factories.SelectMany(i => i.Value)
                .Where(i => i.Value.Scope.ReadyToResolve(IsRoot, container))
                .Select(i => i.Key)
                .Distinct()
                .Union(!IsRoot ? _parentContainer.GetRegistrations(this) : Enumerable.Empty<IRegistration>());
        }

        public IRegistration Register(
            Type stateType,
            Type contractType,
            Func<IResolvingContext, object> factoryMethod,
            object key = null)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            return Register(this, stateType, contractType, factoryMethod, key);
        }

        public IRegistration Register(
            IContainer registerContainer,
            Type stateType,
            Type contractType,
            Func<IResolvingContext, object> factoryMethod,
            object key = null)
        {
            if (registerContainer == null) throw new ArgumentNullException(nameof(registerContainer));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            var comparer = GetComparer();
            Dictionary dictionary;
            if (!_factories.TryGetValue(comparer, out dictionary))
            {
                dictionary = new Dictionary(comparer);
                _factories.Add(comparer, dictionary);
            }

            var resources = new CompositeDisposable();
            var scope = (IScope)Resolve(typeof(EmptyState), typeof(IScope), EmptyState.Shared);
            var registrationDescription = new RegistrationDescription(stateType, contractType, key, resources);
            _chache.Remove(registrationDescription);
            var registration = new StrictRegistration(registrationDescription);
            try
            {
                if (contractType != typeof(ILifetime))
                {
                    if (scope.ReadyToRegister(IsRoot, this))
                    {
                        var lifetime = (ILifetime)Resolve(typeof(EmptyState), typeof(ILifetime), EmptyState.Shared);
                        dictionary.Add(registration, new RegistrationInfo(registerContainer, ctx => lifetime.Create(ctx, factoryMethod), registration, scope));
                        resources.Add(Disposable.Create(() => Unregister(new ReleasingContext(registration), lifetime)));
                    }
                    else
                    {
                        if (!IsRoot)
                        {
                            return _parentContainer.Register(registerContainer, stateType, contractType, factoryMethod, key);
                        }

                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    dictionary.Add(registration, new RegistrationInfo(registerContainer, factoryMethod, registration, scope));
                    resources.Add(Disposable.Create(() => Unregister(registration)));
                }
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"The entry {registration} has already registered. Registered entries:{Environment.NewLine}{GetRegisteredInfo()}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The entry {registration} registration failed. Registered entries:{Environment.NewLine}{GetRegisteredInfo()}", ex);
            }

            return registration;
        }

        public object Resolve(Type stateType, Type contractType, object state, object key = null)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            return Resolve(this, stateType, contractType, state, key);
        }

        public object Resolve(
            IContainer resolverContainer,
            Type stateType,
            Type contractType,
            object state,
            object key = null)
        {
            if (resolverContainer == null) throw new ArgumentNullException(nameof(resolverContainer));
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            var registrationDescription = new RegistrationDescription(stateType, contractType, key, Disposable.Empty());

            RegistrationInfo info;
            if (!_chache.TryGetValue(registrationDescription, out info))
            {
                RegistrationInfo curRegistrationInfo = null;

                info = (
                    from registration in GetResolverRegistrations(registrationDescription)
                    from dictionary in _factories
                    where dictionary.Value.TryGetValue(registration, out curRegistrationInfo)
                    where curRegistrationInfo.Scope.ReadyToResolve(IsRoot, this)
                    select new RegistrationInfo(curRegistrationInfo.RegisterContainer, curRegistrationInfo.Factory, registration, curRegistrationInfo.Scope)).FirstOrDefault() ?? new RegistrationInfo();

                _chache.Add(registrationDescription, info);
            }

            Exception innerException = null;
            if (!info.IsEmpty && info.Scope.ReadyToResolve(IsRoot, resolverContainer))
            {
                using (var ctx = new ResolvingContext(info.RegisterContainer, resolverContainer, info.Registration, contractType, state))
                {
                    return info.Factory(ctx);
                }
            }

            try
            {
                if (!IsRoot)
                {
                    return _parentContainer.Resolve(resolverContainer, stateType, contractType, state, key);
                }

                // Defaults
                object defaultInstance;
                if (stateType == typeof(EmptyState) && key == null && DefaultInstances.TryGetValue(contractType, out defaultInstance))
                {
                    return defaultInstance;
                }
            }
            catch (InvalidOperationException ex)
            {
                innerException = ex;
            }

            var keys = string.Join(" or ", GetResolverRegistrations(registrationDescription).Select(i => i.ToString()));
            throw new InvalidOperationException($"The entries {keys} was not registered. {GetRegisteredInfo()}", innerException);
        }

        /// <summary>
        /// Disposes this container contract and any child containers. Also disposes any registered object contracts whose lifetimes are managed by the resolver.
        /// </summary>
        public void Dispose()
        {
            _chache.Clear();
            _factories.SelectMany(i => i.Value.Keys).Distinct().ToCompositeDisposable().Dispose();
            _disposable.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(Container)} [Key: {Key?.ToString() ?? string.Empty}, IsRoot: {IsRoot}, Registrations: {_factories.SelectMany(i => i.Value).Count()}]";
        }

        private bool Unregister(IRegistration registration)
        {
            _chache.Remove(
                new RegistrationDescription(
                    registration.StateType,
                    registration.ContractType,
                    registration.Key,
                    Disposable.Empty()));

            var removed = false;
            foreach (var dictionary in _factories)
            {
                removed |= dictionary.Value.Remove(registration);
            }

            return removed;
        }

        private bool Unregister(IReleasingContext ctx, ILifetime factory)
        {
            if (Unregister(ctx.Registration))
            {
                factory?.Release(ctx);
                return true;
            }

            return false;
        }

        private string GetRegisteredInfo()
        {
            var details = _factories.Count == 0 ? "no entries" : string.Join(Environment.NewLine, _factories.SelectMany(i => i.Value.Keys).Distinct().Select(k => k.ToString()));
            return $"Container \"{Key}\". Registered entries:{Environment.NewLine}{details}";
        }

        private static IEnumerable<IRegistration> GetResolverRegistrations(RegistrationDescription registrationDescription)
        {
            yield return new StrictRegistration(registrationDescription);

            IRegistration genericRegistration;
            if (GenericRegistration.TryCreate(registrationDescription, out genericRegistration))
            {
                yield return genericRegistration;
            }
        }

        private IRegistrationComparer GetComparer()
        {
            return (IRegistrationComparer)Resolve(typeof(EmptyState), typeof(IRegistrationComparer), EmptyState.Shared);
        }

        internal class RegistrationInfo
        {
            public RegistrationInfo()
            {
                IsEmpty = true;
            }

            public RegistrationInfo(IContainer registerContainer, Func<IResolvingContext, object> factory, IRegistration registration, IScope scope)
            {
                RegisterContainer = registerContainer;
                Factory = factory;
                Registration = registration;
                Scope = scope;
            }

            public bool IsEmpty { get; }

            public IContainer RegisterContainer { get; }

            public Func<IResolvingContext, object> Factory { get; }

            public IRegistration Registration { get; }

            public IScope Scope { get; }
        }
    }
}
