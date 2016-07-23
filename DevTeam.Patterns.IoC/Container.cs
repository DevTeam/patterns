namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Dispose;
    using Dictionary = System.Collections.Generic.Dictionary<IRegistration, System.Func<IResolvingContext, object>>;

    public class Container: IContainer
	{
        private static readonly ComparerForRegistrationComparer ComparerForRegistrationComparer = new ComparerForRegistrationComparer();
        private readonly SortedDictionary<IRegistrationComparer, Dictionary> _factories = new SortedDictionary<IRegistrationComparer, Dictionary>(ComparerForRegistrationComparer);
		private readonly IContainer _parentContainer;
        private readonly IDisposable _disposable = Disposable.Empty();

        /// <summary>
        /// Creates a default container with key/name. Returns a reference to the new container.
        /// </summary>
        /// <param name="key">Container's key. For example a name.</param>
        public Container(object key = null)
        {
            Key = key;
            _disposable = ContainerConfiguration.Shared.CreateRegistrations(this).ToCompositeDisposable();
        }

        internal Container(ContainerDescription containerDescription)
        {
            if (containerDescription == null) throw new ArgumentNullException(nameof(containerDescription));

            Key = containerDescription.Key;
            _parentContainer = containerDescription.ParentContainer;
        }        

        public object Key { get; }

	    public IEnumerable<IRegistration> Registrations => _factories.SelectMany(i => i.Value.Keys).Distinct().Union(_parentContainer != null ? _parentContainer.Registrations : Enumerable.Empty<IRegistration>());

	    public IRegistration Register(Type stateType, Type instanceType, Func<IResolvingContext, object> factoryMethod, object key = null)
		{
		    if (stateType == null) throw new ArgumentNullException(nameof(stateType));
		    if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
		    if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));

            IRegistrationComparer comparer;
            if (!TryGetComparer(out comparer))
            {
                comparer = ContainerConfiguration.RootContainerRegestryKeyComparer.Value;
            }

            Dictionary dictionary;
            if (!_factories.TryGetValue(comparer, out dictionary))
            {
                dictionary = new Dictionary(comparer);
                _factories.Add(comparer, dictionary);
            }

            var resources = new CompositeDisposable();
            var registrationDescription = new RegistrationDescription(stateType, instanceType, key, resources);
	        var registration = new StrictRegistration(registrationDescription);
            try
            {
                if (instanceType != typeof(ILifetime))
	            {
	                var lifetime = (ILifetime)Resolve(typeof(EmptyState), typeof(ILifetime), EmptyState.Shared);
                    dictionary.Add(registration, ctx => lifetime.Create(ctx, factoryMethod));
                    resources.Add(Disposable.Create(() => Unregister(new ReleasingContext(registration), lifetime)));
	            }
	            else
	            {
                    dictionary.Add(registration, factoryMethod);
	                resources.Add(Disposable.Create(() => Unregister(registration)));
	            }	            
	        }
	        catch (Exception ex)
	        {
	            throw new InvalidOperationException($"The entry {registration} registration failed. Registered entries:\n{GetRegisteredInfo()}", ex);
	        }
	        
	        return registration;
		}

	    public object Resolve(Type stateType, Type instanceType, object state, object key = null)
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));

            var registrationDescription = new RegistrationDescription(stateType, instanceType, key, Disposable.Empty());
	        foreach (var registration in GetResolverRegistrations(registrationDescription))
	        {
	            foreach (var dictionary in _factories)
	            {
	                Func<IResolvingContext, object> factory;
	                if (dictionary.Value.TryGetValue(registration, out factory))
                    {
                        using (var ctx = new ResolvingContext(this, registration, instanceType, state))
                        {
                            return factory(ctx);
                        }
                    }
	            }
	        }

	        Exception innerException = null;
		    try
		    {
		        if (_parentContainer != null)
		        {
		            return _parentContainer.Resolve(stateType, instanceType, state, key);
		        }
		        
                // Defaults		      
		        if (instanceType == typeof(ILifetime) && stateType == typeof(EmptyState) && key == null)
		        {
                    return ContainerConfiguration.TransientLifetime.Value;
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
        /// Disposes this container instance and any child containers. Also disposes any registered object instances whose lifetimes are managed by the container.
        /// </summary>
        public void Dispose()
	    {
            _factories.SelectMany(i => i.Value.Keys).Distinct().ToCompositeDisposable().Dispose();
            _disposable.Dispose();
        }

	    public override string ToString()
	    {
	        return Key?.ToString() ?? string.Empty;
	    }	    

	    private bool Unregister(IRegistration registration)
	    {
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

        private bool TryGetComparer(out IRegistrationComparer comparer)
        {
            var comparers = (
                from registration in Registrations
                where registration.InstanceType == typeof(IRegistrationComparer) && registration.StateType == typeof(EmptyState) && registration.Key == null
                select (IRegistrationComparer)Resolve(registration.StateType, registration.InstanceType, EmptyState.Shared, registration.Key)).ToList();

            if (comparers.Count == 1)
            {
                comparer = comparers[0];
                return true;
            }

            comparer = ContainerConfiguration.RootContainerRegestryKeyComparer.Value;
            return true;
        }
    }
}
