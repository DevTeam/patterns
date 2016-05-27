namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;

    public class Container : IContainer
	{
	    private readonly Dictionary<Key, Func<object, object>> _factories = new Dictionary<Key, Func<object, object>>();
		private readonly Container _parentContainer;

		public Container(string name = "")
		{
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    Name = name;
		    UseContext = true;
		}       

        public string Name { get; }

	    internal bool UseContext { get; set; }

	    private Container(Container parentContainer, string name)
			:this(name)
		{
		    if (parentContainer == null) throw new ArgumentNullException(nameof(parentContainer));

		    _parentContainer = parentContainer;
		}        			
		
		public IDisposable Register(Type stateType, Type instanceType, Func<object, object> factoryMethod, string name = "")
		{
		    if (stateType == null) throw new ArgumentNullException(nameof(stateType));
		    if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
		    if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    var key = new Key(stateType, instanceType, name);            
		    try
		    {
		        var factoryMethodToRegister = factoryMethod;

                if (UseContext)
		        {
		            var factory = Context.Instance.Resolve<ILifetime>();
                    factoryMethodToRegister = state => factory.Create(factoryMethod, state);
		        }

                _factories.Add(key, factoryMethodToRegister);
		        return Disposable.Create(() => _factories.Remove(key));
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
		        return new Container(this, name) { UseContext = UseContext };
		    }

            var key = new Key(stateType, instanceType, name);
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
	    }

	    public override string ToString()
	    {
	        return Name;
	    }

	    private string GetRegisteredInfo()
	    {
	        var details = _factories.Count == 0 ? "no entries" : string.Join(", ", _factories.Keys.Select(k => k.ToString()));
            return $"Container \"{Name}\". Registered entries: {details}";
	    }        

        private class Key
        {
            private readonly Type _instanceType;
            private readonly Type _stateType;            
            private readonly string _name;

            public Key(Type stateType, Type instanceType, string name)
            {
                if (stateType == null) throw new ArgumentNullException(nameof(stateType));
                if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
                if (name == null) throw new ArgumentNullException(nameof(name));

                _instanceType = instanceType;
                _stateType = stateType;                
                _name = name;                
            }

            public override string ToString()
            {
                return $"{_instanceType.FullName}({_stateType.FullName}, \"{_name}\")";
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Key)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _instanceType.GetHashCode();
                    hashCode = (hashCode * 397) ^ _stateType.GetHashCode();
                    hashCode = (hashCode * 397) ^ _name.GetHashCode();
                    return hashCode;
                }
            }

            private bool Equals(Key other)
            {
                return _instanceType == other._instanceType && _stateType == other._stateType && string.Equals(_name, other._name);
            }
        }
	}
}
