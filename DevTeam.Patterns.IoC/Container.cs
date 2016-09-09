namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Dispose;
    using Dictionary = System.Collections.Generic.Dictionary<IRegistration, Container.RegistrationInfo>;

    internal class Container : IContainer
    {
        private static readonly Dictionary<IRegistration, Func<IResolvingContext, object>> Defaults = new Dictionary<IRegistration, Func<IResolvingContext, object>>
        {
            { new Registration(typeof(EmptyState), typeof(ILifetime), null), ctx => RootContainerConfiguration.TransientLifetime },
            { new Registration(typeof(EmptyState), typeof(IComparer), null), ctx => RootContainerConfiguration.FullComplianceComparer },
            { new Registration(typeof(EmptyState), typeof(IBinder), null), ctx => RootContainerConfiguration.Binder },
            { new Registration(typeof(EmptyState), typeof(IFactory), null), ctx => RootContainerConfiguration.Factory },
            { new Registration(typeof(EmptyState), typeof(IScope), null), ctx => RootContainerConfiguration.PublicScope },
            { new Registration(typeof(EmptyState), typeof(IContractRange), null), ctx => RootContainerConfiguration.ContractRange }
        };

        private static readonly TypeInfo ContextTypeInfo = typeof(IContext).GetTypeInfo();
        private static readonly ComparerForRegistrationComparer ComparerForRegistrationComparer = new ComparerForRegistrationComparer();
        private readonly SortedDictionary<IComparer, Dictionary> _factories = new SortedDictionary<IComparer, Dictionary>(ComparerForRegistrationComparer);
        private readonly Dictionary<IRegistration, RegistrationInfo> _cache = new Dictionary<IRegistration, RegistrationInfo>();
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

        public IEnumerable<IRegistration> GetRegistrations(IContainerContext containerContext)
        {
            if (containerContext == null) throw new ArgumentNullException(nameof(containerContext));

            return 
                _factories.SelectMany(i => i.Value)
                .Where(i => i.Value.Scope.ReadyToResolve(IsRoot, containerContext.TargetContainer))
                .Select(i => i.Key)
                .Distinct()
                .Union(!IsRoot ? _parentContainer.GetRegistrations(containerContext) : Enumerable.Empty<IRegistration>());
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

            return Register(this, new Registration(stateType, contractType, key), factoryMethod);
        }

        public IRegistration Register(
            IContainer registerContainer,
            IRegistration registration,
            Func<IResolvingContext, object> factoryMethod)
        {
            if (registerContainer == null) throw new ArgumentNullException(nameof(registerContainer));
            if (registration == null) throw new ArgumentNullException(nameof(registration));
            if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            var comparer = GetComparer(registration);
            Dictionary dictionary;
            if (!_factories.TryGetValue(comparer, out dictionary))
            {
                dictionary = new Dictionary(comparer);
                _factories.Add(comparer, dictionary);
            }

            var resources = new CompositeDisposable();

            try
            {
                if (registration.ContractType != typeof(ILifetime))
                {
                    var scope = (IScope)Resolve(typeof(EmptyState), typeof(IScope), EmptyState.Shared);
                    if (scope.ReadyToRegister(IsRoot, this))
                    {
                        var lifetime = (ILifetime)Resolve(typeof(EmptyState), typeof(ILifetime), EmptyState.Shared);
                        var contractRange = (IContractRange)Resolve(typeof(EmptyState), typeof(IContractRange), EmptyState.Shared);
                        foreach (var registrationVariant in contractRange.GetRegisterVariants(registration))
                        {
                            _cache.Remove(registrationVariant);
                            dictionary.Add(registrationVariant, new RegistrationInfo(registerContainer, ctx => lifetime.Create(ctx, factoryMethod), registrationVariant, scope));
                            resources.Add(Disposable.Create(() => Unregister(new ReleasingContext(registrationVariant), lifetime)));
                        }
                    }
                    else
                    {
                        if (!IsRoot)
                        {
                            return _parentContainer.Register(registerContainer, registration, factoryMethod);
                        }

                        throw new InvalidOperationException();
                    }
                }
                else
                {
                    _cache.Remove(registration);
                    dictionary.Add(registration, new RegistrationInfo(registerContainer, factoryMethod, registration, RootContainerConfiguration.PublicScope));
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

            return new Registration(registration, resources);
        }

        public object Resolve(Type stateType, Type contractType, object state, object key = null)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            return Resolve(this, new Registration(stateType, contractType, key), state);
        }

        public object Resolve(
            IContainer resolveContainer,
            IRegistration registration,
            object state)
        {
            if (resolveContainer == null) throw new ArgumentNullException(nameof(resolveContainer));
            if (registration == null) throw new ArgumentNullException(nameof(registration));

            RegistrationInfo info;
            if (!_cache.TryGetValue(registration, out info))
            {
                RegistrationInfo curRegistrationInfo = null;

                info = (
                    from registrationVariant in registration.GetResolveVariants()
                    from dictionary in _factories
                    where dictionary.Value.TryGetValue(registrationVariant, out curRegistrationInfo)
                    where curRegistrationInfo.Scope.ReadyToResolve(IsRoot, this)
                    select new RegistrationInfo(curRegistrationInfo.RegisterContainer, curRegistrationInfo.Factory, registrationVariant, curRegistrationInfo.Scope)).FirstOrDefault() ?? new RegistrationInfo();

                _cache.Add(registration, info);
            }

            Exception innerException = null;
            if (!info.IsEmpty && info.Scope.ReadyToResolve(IsRoot, resolveContainer))
            {
                using (var ctx = new ResolvingContext(info.RegisterContainer, resolveContainer, info.Registration, registration.ContractType, state))
                {
                    return info.Factory(ctx);
                }
            }

            try
            {
                if (!IsRoot)
                {
                    return _parentContainer.Resolve(resolveContainer, registration, state);
                }

                // Defaults
                Func<IResolvingContext, object> factory;
                if (Defaults.TryGetValue(registration, out factory))
                {
                    return factory(new ResolvingContext(info.RegisterContainer, resolveContainer, info.Registration, registration.ContractType, state));
                }
            }
            catch (InvalidOperationException ex)
            {
                innerException = ex;
            }

            throw new InvalidOperationException($"The entries {registration} was not registered. {GetRegisteredInfo()}", innerException);
        }

        /// <summary>
        /// Disposes this container contract and any child containers. Also disposes any registered object contracts whose lifetimes are managed by the resolver.
        /// </summary>
        public void Dispose()
        {
            _cache.Clear();
            _factories.SelectMany(i => i.Value.Keys).Distinct().ToCompositeDisposable().Dispose();
            _disposable.Dispose();
        }

        public override string ToString()
        {
            return $"{nameof(Container)} [Key: {Key?.ToString() ?? string.Empty}, IsRoot: {IsRoot}, Registrations: {_factories.SelectMany(i => i.Value).Count()}]";
        }

        private bool Unregister(IRegistration registration)
        {
            _cache.Remove(registration);

            var removed = false;
            foreach (var dictionary in _factories)
            {
                removed |= dictionary.Value.Remove(registration);
            }

            return removed;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
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
            return $"Container [Key: {Key?.ToString() ?? "null"}, Registered entries: {Environment.NewLine}{details}]";
        }

        private IComparer GetComparer(IRegistration registration)
        {
            if (ContextTypeInfo.IsAssignableFrom(registration.ContractType.GetTypeInfo()))
            {
                return RootContainerConfiguration.FullComplianceComparer;
            }

            return (IComparer)Resolve(typeof(EmptyState), typeof(IComparer), EmptyState.Shared);
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
