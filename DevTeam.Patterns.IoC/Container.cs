namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;

    public class Container: IContainer
	{
        private Dictionary<IRegistration, Func<IResolvingContext, object>> _factories;
		private readonly IContainer _parentContainer;
        private readonly IDisposable _disposable = Disposable.Empty();

        /// <summary>
        /// Creates root container.
        /// </summary>
        /// <param name="key">Container's key. For example a name.</param>
        public Container(object key = null)
        {
            Key = key;
            CreateFactories();
            _disposable = IoCContainerConfiguration.Shared.CreateRegistrations(this).ToCompositeDisposable();
        }

        internal Container(ContainerDescription containerDescription)
        {
            if (containerDescription == null) throw new ArgumentNullException(nameof(containerDescription));

            Key = containerDescription.Key;
            _parentContainer = containerDescription.ParentContainer;
            CreateFactories();
        }        

        public object Key { get; }

	    public IEnumerable<IRegistration> Registrations => _factories.Keys.Union(_parentContainer != null ? _parentContainer.Registrations : Enumerable.Empty<IRegistration>());

	    public IRegistration Register(Type stateType, Type instanceType, Func<IResolvingContext, object> factoryMethod, object key = null)
		{
		    if (stateType == null) throw new ArgumentNullException(nameof(stateType));
		    if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
		    if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));		    

            var resources = new CompositeDisposable();
            var registrationDescription = new RegistrationDescription(stateType, instanceType, key, resources);
	        var registration = new StrictRegistration(registrationDescription);
            try
	        {
	            if (instanceType != typeof(ILifetime))
	            {
	                var lifetime = (ILifetime)Resolve(typeof(EmptyState), typeof(ILifetime), EmptyState.Shared);
                    _factories.Add(registration, ctx => lifetime.Create(ctx, factoryMethod));
                    resources.Add(Disposable.Create(() => Unregister(new ReleasingContext(registration), lifetime)));
	            }
	            else
	            {
	                _factories.Add(registration, factoryMethod);
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
	            Func<IResolvingContext, object> factory;
	            if (_factories.TryGetValue(registration, out factory))
                {
                    using (var ctx = new ResolvingContext(this, registration, instanceType, state))
                    {
                        return factory(ctx);
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
                    return IoCContainerConfiguration.TransientLifetime.Value;
		        }
            }
		    catch (InvalidOperationException ex)
		    {
		        innerException = ex;                
            }

	        var keys = string.Join(" or ", GetResolverRegistrations(registrationDescription).Select(i => i.ToString()));
            throw new InvalidOperationException($"The entries {keys} was not registered. {GetRegisteredInfo()}", innerException);
        }
        	
		public void Dispose()
	    {
            _factories.Keys.ToCompositeDisposable().Dispose();
            _disposable.Dispose();
        }

	    public override string ToString()
	    {
	        return Key?.ToString() ?? string.Empty;
	    }	    

	    private bool Unregister(IRegistration registration)
        {
            return _factories.Remove(registration);
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
	        var details = _factories.Count == 0 ? "no entries" : string.Join(", ", _factories.Keys.Select(k => k.ToString()));
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

        private void CreateFactories()
        {
            IEqualityComparer<IRegistration> comparer;
            _factories = TryGetComparer(out comparer)
                ? new Dictionary<IRegistration, Func<IResolvingContext, object>>(comparer)
                : new Dictionary<IRegistration, Func<IResolvingContext, object>>();
        }

        private bool TryGetComparer(out IEqualityComparer<IRegistration> comparer)
        {
            if (_parentContainer == null)
            {
                comparer = IoCContainerConfiguration.RootContainerRegestryKeyComparer.Value;
                return true;
            }

            var comparers = (
                from registration in _parentContainer.Registrations
                where registration.InstanceType == typeof(IRegistrationComparer) && registration.StateType == typeof(EmptyState) && registration.Key == null
                select (IRegistrationComparer)_parentContainer.Resolve(registration.StateType, registration.InstanceType, EmptyState.Shared, registration.Key)).ToList();

            if (comparers.Count == 1)
            {
                comparer = comparers[0];
                return true;
            }

            comparer = default(IEqualityComparer<IRegistration>);
            return true;
        }
    }
}
