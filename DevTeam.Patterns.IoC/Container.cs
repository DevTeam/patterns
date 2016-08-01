namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;
    using Dictionary = System.Collections.Generic.Dictionary<IRegistration, Container.RegistrationInfo>;

    public class Container: IContainer
	{
        private static readonly Dictionary<Type, object> DefaultInstances = new Dictionary<Type, object>
        {
            { typeof(ILifetime), RootContainerConfiguration.TransientLifetime.Value },
            { typeof(IRegistrationComparer), RootContainerConfiguration.RootContainerRegestryKeyComparer.Value },
            { typeof(IBinder), RootContainerConfiguration.Binder.Value },
            { typeof(IFactory), RootContainerConfiguration.Factory.Value },
            { typeof(IScope), RootContainerConfiguration.PublicScope.Value }
        };

        private static readonly ComparerForRegistrationComparer ComparerForRegistrationComparer = new ComparerForRegistrationComparer();
        private readonly SortedDictionary<IRegistrationComparer, Dictionary> _factories = new SortedDictionary<IRegistrationComparer, Dictionary>(ComparerForRegistrationComparer);
        private readonly Dictionary<RegistrationDescription, RegistrationInfo> _chache = new Dictionary<RegistrationDescription, RegistrationInfo>();
        private readonly IResolver _parentResolver;
        private readonly IDisposable _disposable = Disposable.Empty();

        /// <summary>
        /// Creates a default container with key/name. Returns a reference to the new container.
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
            _parentResolver = containerDescription.ParentResolver;
        }

        public object Key { get; }

	    public IEnumerable<IRegistration> Registrations => 
            _factories
            .SelectMany(i => i.Value)
            .Where(i => i.Value.Scope.Satisfy(this))
            .Select(i => i.Key)
            .Distinct()
            .Union(_parentResolver != null ? _parentResolver.Registrations : Enumerable.Empty<IRegistration>());

	    public IRegistration Register(Type stateType, Type contractType, Func<IResolvingContext, object> factoryMethod, object key = null)
		{
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
            var scope = (IScope)Resolve(this, typeof(IResolver), typeof(IScope), this);
            var registrationDescription = new RegistrationDescription(stateType, contractType, key, resources);
	        _chache.Remove(registrationDescription);
            var registration = new StrictRegistration(registrationDescription);
            try
            {
                if (contractType != typeof(ILifetime))
	            {
	                var lifetime = (ILifetime)Resolve(this, typeof(EmptyState), typeof(ILifetime), EmptyState.Shared);
                    dictionary.Add(registration, new RegistrationInfo(ctx => lifetime.Create(ctx, factoryMethod), registration, scope));
                    resources.Add(Disposable.Create(() => Unregister(new ReleasingContext(registration), lifetime)));
	            }
	            else
	            {
                    dictionary.Add(registration, new RegistrationInfo(factoryMethod, registration, scope));
	                resources.Add(Disposable.Create(() => Unregister(registration)));
	            }	            
	        }
	        catch (Exception ex)
	        {
	            throw new InvalidOperationException($"The entry {registration} registration failed. Registered entries:\n{GetRegisteredInfo()}", ex);
	        }
	        
	        return registration;
		}

	    public object Resolve(IResolver resolver, Type stateType, Type contractType, object state, object key = null)
        {
	        if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (contractType == null) throw new ArgumentNullException(nameof(contractType));

            resolver = resolver ?? this;
            var registrationDescription = new RegistrationDescription(stateType, contractType, key, Disposable.Empty());

            RegistrationInfo info;
	        if (!_chache.TryGetValue(registrationDescription, out info))
	        {
                RegistrationInfo curRegistrationInfo = null;

                info = (
                    from registration in GetResolverRegistrations(registrationDescription)
                    from dictionary in _factories
                    where dictionary.Value.TryGetValue(registration, out curRegistrationInfo)
                    where curRegistrationInfo.Scope.Satisfy(resolver)
                    select new RegistrationInfo(curRegistrationInfo.Factory, registration, curRegistrationInfo.Scope)).FirstOrDefault() ?? new RegistrationInfo();
            }

	        if (!info.IsEmpty)
	        {
	            using (var ctx = new ResolvingContext(resolver, info.Registration, contractType, state))
	            {
	                return info.Factory(ctx);
	            }
	        }

	        Exception innerException = null;
		    try
		    {
		        if (_parentResolver != null)
		        {
		            return _parentResolver.Resolve(resolver, stateType, contractType, state, key);
		        }
		        
                // Defaults		      
		        object defaultInstance;
		        if (key == null && DefaultInstances.TryGetValue(contractType, out defaultInstance))
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
        /// Disposes this container contract and any child containers. Also disposes any registered object contracts whose lifetimes are managed by the container.
        /// </summary>
        public void Dispose()
	    {
            _chache.Clear();
            _factories.SelectMany(i => i.Value.Keys).Distinct().ToCompositeDisposable().Dispose();
            _disposable.Dispose();
        }

	    public override string ToString()
	    {
	        return Key?.ToString() ?? string.Empty;
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
            var details = _factories.Count == 0 ? "no entries" : string.Join(", ", _factories.SelectMany(i => i.Value.Keys).Distinct().Select(k => k.ToString()));
            return $"Container \"{Key}\". Registered entries: {details}";
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
            return (IRegistrationComparer)Resolve(this, typeof(EmptyState), typeof(IRegistrationComparer), EmptyState.Shared, null);            
        }

        internal class RegistrationInfo
        {
            public RegistrationInfo()
            {
                IsEmpty = true;
            }

            public RegistrationInfo(Func<IResolvingContext, object> factory, IRegistration registration, IScope scope)
            {
                Factory = factory;
                Registration = registration;
                Scope = scope;
            }

            public bool IsEmpty { get; }

            public Func<IResolvingContext, object> Factory { get; }

            public IRegistration Registration { get; }

            public IScope Scope { get; }
        }
    }
}
