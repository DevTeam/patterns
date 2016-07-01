namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;

    public class Container: IContainer
	{
        private static readonly IConfiguration Configuration = new IoCContainerConfiguration();
        private readonly Dictionary<IKey, Func<Type, object, object>> _factories = new Dictionary<IKey, Func<Type, object, object>>();
		private readonly IContainer _parentContainer;
        
        /// <summary>
        /// Creates root container.
        /// </summary>
        /// <param name="name"></param>
	    public Container(string name = "")
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
            Configuration.Apply(this);            
        }

        internal Container(ContainerDescription containerDescription)
        {
            if (containerDescription == null) throw new ArgumentNullException(nameof(containerDescription));

            Name = containerDescription.Name;
            _parentContainer = containerDescription.ParentContainer;
        }

        public string Name { get; }

	    public IEnumerable<IKey> Keys => _factories.Keys.Union(_parentContainer != null ? _parentContainer.Keys : Enumerable.Empty<IKey>());

	    public IDisposable Register(Type stateType, Type instanceType, Func<Type, object, object> factoryMethod, string name = "")
		{
		    if (stateType == null) throw new ArgumentNullException(nameof(stateType));
		    if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
		    if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
		    if (name == null) throw new ArgumentNullException(nameof(name));

            var registration = new CompositeDisposable();
            var resources = new CompositeDisposable();
            var keyDescription = new KeyDescription(stateType, instanceType, name, resources);
	        foreach (var key in GetRegisterKeys(keyDescription))
	        {
	            try
	            {
	                if (instanceType != typeof(ILifetime))
	                {
	                    var lifetime = (ILifetime)Resolve(typeof(EmptyState), typeof(ILifetime), EmptyState.Shared);
	                    _factories.Add(key, (type, state) => lifetime.Create(this, key, factoryMethod, type, state));
	                    resources.Add(Disposable.Create(() => Unregister(key, lifetime)));
	                }
	                else
	                {
	                    _factories.Add(key, factoryMethod);
	                    resources.Add(Disposable.Create(() => Unregister(key)));
	                }

	                var disposableKey = key as IDisposable;
	                if (disposableKey != null)
	                {
	                    registration.Add(disposableKey);
	                }
	            }
	            catch (Exception ex)
	            {
	                throw new InvalidOperationException(
	                    $"The entry {key} registration failed. Registered entries:\n{GetRegisteredInfo()}",
	                    ex);
	            }
	        }

	        return registration;
		}

	    public object Resolve(Type stateType, Type instanceType, object state, string name = "")
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (instanceType == typeof(IContainer) && stateType == typeof(EmptyState))
            {
                return (IContainer)Resolve(typeof(ContainerDescription), typeof(IContainer), new ContainerDescription(this, name));
            }

            var keyDescription = new KeyDescription(stateType, instanceType, name, Disposable.Empty());
	        foreach (var key in GetResolveKeys(keyDescription))
	        {
	            Func<Type, object, object> factory;
	            if (_factories.TryGetValue(key, out factory))
                {
                    return factory(instanceType, state);
                }
	        }

	        Exception innerException = null;
		    try
		    {
		        if (_parentContainer != null)
		        {
		            return _parentContainer.Resolve(stateType, instanceType, state, name);
		        }
		        
                // Defaults		      
		        if (instanceType == typeof(ILifetime) && stateType == typeof(EmptyState) && name == string.Empty)
		        {
                    return IoCContainerConfiguration.TransientLifetime.Value;
		        }
            }
		    catch (InvalidOperationException ex)
		    {
		        innerException = ex;                
            }

	        var keys = string.Join(" or ", GetResolveKeys(keyDescription).Select(i => i.ToString()));
            throw new InvalidOperationException($"The entry {keys} was not registered. {GetRegisteredInfo()}", innerException);
        }
        	
		public void Dispose()
	    {
	        var disposableKeys = new List<IDisposable>(_factories.Keys.OfType<IDisposable>());
            foreach (var disposableKey in disposableKeys)
	        {
                disposableKey.Dispose();           
            }
	    }

	    public override string ToString()
	    {
	        return Name;
	    }

	    internal IEnumerable<IKey> Registrations => _factories.Keys;

	    private bool Unregister(IKey key)
        {
            return _factories.Remove(key);
        }

        private bool Unregister(IKey key, ILifetime factory)
        {
            if (Unregister(key))
            {
                factory?.Release(this, key);
                return true;
            }

            return false;
        }

        private string GetRegisteredInfo()
	    {
	        var details = _factories.Count == 0 ? "no entries" : string.Join(", ", _factories.Keys.Select(k => k.ToString()));
            return $"Container \"{Name}\". Registered entries: {details}";
	    }

        private IEnumerable<IKey> GetRegisterKeys(KeyDescription keyDescription)
        {
            yield return new StrictKey(keyDescription);
        }

        private IEnumerable<IKey> GetResolveKeys(KeyDescription keyDescription)
        {
            yield return new StrictKey(keyDescription);
            yield return new GenericKey(keyDescription);
        }
    }
}
