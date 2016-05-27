namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;

    public class Container : IContainer
	{
	    private readonly bool _useContext;
	    private readonly Dictionary<IRegistryKey, Func<object, object>> _factories = new Dictionary<IRegistryKey, Func<object, object>>();
		private readonly Container _parentContainer;

	    public Container(string name = "")
	        : this(name, true)
	    {
	    }

	    internal Container(string name, bool useContext)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _useContext = useContext;
            Name = name;
        }

        private Container(Container parentContainer, string name, bool useContext)
			:this(name, useContext)
		{
		    if (parentContainer == null) throw new ArgumentNullException(nameof(parentContainer));

		    _parentContainer = parentContainer;
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
		        var factoryMethodToRegister = factoryMethod;

                ILifetime factory = null;
                if (_useContext)
		        {
                    factory = Context.Instance.Resolve<ILifetime>();
                    factoryMethodToRegister = state => factory.Create(key, factoryMethod, state);
		        }

                _factories.Add(key, factoryMethodToRegister);
                resources.Add(Disposable.Create(() => Unregister(key, factory)));
                return resources;
		    }
		    catch (Exception ex)
		    {
                throw new InvalidOperationException($"The entry {key} was alredy registered. Registered entries:\n{GetRegisteredInfo()}", ex);
            }		    
		}

	    public object Resolve(Type stateType, Type instanceType, object state, string name = "")
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (instanceType == typeof(IContainer))
		    {
		        return new Container(this, name, _useContext);
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

        private void Unregister(IRegistryKey key, ILifetime factory)
        {
            if (_factories.Remove(key) && _useContext)
            {
                factory?.Release(key);
            }
        }

        private string GetRegisteredInfo()
	    {
	        var details = _factories.Count == 0 ? "no entries" : string.Join(", ", _factories.Keys.Select(k => k.ToString()));
            return $"Container \"{Name}\". Registered entries: {details}";
	    }
	}
}
