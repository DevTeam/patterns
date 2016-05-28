namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;

    public class Container : IContainer
    {
        private static readonly IConfiguration Configuration = new IoCContainerConfiguration();
        private readonly Dictionary<IRegistryKey, Func<object, object>> _factories = new Dictionary<IRegistryKey, Func<object, object>>();
		private readonly IContainer _parentContainer;
        
        /// <summary>
        /// Creates root container.
        /// </summary>
        /// <param name="name"></param>
	    public Container(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            Name = name;
            Configuration.Apply(this);            
        }

        internal Container(ContainerInfo containerInfo)
        {
            if (containerInfo == null) throw new ArgumentNullException(nameof(containerInfo));

            Name = containerInfo.Name;
            _parentContainer = containerInfo.ParentContainer;
        }

        public string Name { get; }

        public IDisposable Register(Type stateType, Type instanceType, Func<object, object> factoryMethod, string name = "")
		{
		    if (stateType == null) throw new ArgumentNullException(nameof(stateType));
		    if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
		    if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    var resources = new CompositeDisposable();
            var key = new RegistryKey(stateType, instanceType, name, resources);            
		    try
		    {                
		        if (instanceType != typeof(ILifetime))
                {
		            var lifetime = (ILifetime)Resolve(typeof(EmptyState), typeof(ILifetime), EmptyState.Shared);
                    _factories.Add(key, state => lifetime.Create(this, key, factoryMethod, state));
                    resources.Add(Disposable.Create(() => Unregister(key, lifetime)));
                }
                else
                {
                    _factories.Add(key, factoryMethod);
                    resources.Add(Disposable.Create(() => Unregister(key)));
                }
                                
                return key;
		    }
		    catch (Exception ex)
		    {
                throw new InvalidOperationException($"The entry {key} registration failed. Registered entries:\n{GetRegisteredInfo()}", ex);
            }		    
		}

	    public object Resolve(Type stateType, Type instanceType, object state, string name = "")
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (instanceType == typeof(IContainer) && stateType == typeof(EmptyState))
            {
                return (IContainer)Resolve(typeof(ContainerInfo), typeof(IContainer), new ContainerInfo(this, name));
            }

            var key = new RegistryKey(stateType, instanceType, name, Disposable.Empty());
            Func<object, object> factory;
			if (_factories.TryGetValue(key, out factory))
			{                
				return factory(state);				
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
                    return new TransientLifetime();
		        }
		    }
		    catch (InvalidOperationException ex)
		    {
		        innerException = ex;                
            }

            throw new InvalidOperationException($"The entry {key} was not registered. {GetRegisteredInfo()}", innerException);
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

        private bool Unregister(IRegistryKey key)
        {
            return _factories.Remove(key);
        }

        private bool Unregister(IRegistryKey key, ILifetime factory)
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
	}
}
