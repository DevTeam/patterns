namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Container : IContainer
	{
	    private readonly Dictionary<Key, object> _factories = new Dictionary<Key, object>();
		private readonly Container _parentContainer;

		public Container(string name = "")
		{
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    Name = name;            
		}

	    public string Name { get; }

	    private Container(Container parentContainer, string name)
			:this(name)
		{
		    if (parentContainer == null) throw new ArgumentNullException(nameof(parentContainer));

		    _parentContainer = parentContainer;
		}        			
		
		public IRegistry Register<TState, T>(Func<TState, T> factory, string name = "")
		{
		    if (factory == null) throw new ArgumentNullException(nameof(factory));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    var key = new Key(typeof(TState), typeof(T), name);            
		    try
		    {
		        _factories.Add(key, factory);
		    }
		    catch (Exception ex)
		    {
                throw new InvalidOperationException($"The entry {key} was alredy registered. Registered entries:\n{GetRegisteredInfo()}", ex);
            }

		    return this;
		}		

		public T Resolve<TState, T>(TState state, string name = "")
		{
		    if (state == null) throw new ArgumentNullException(nameof(state));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    if (typeof(T) == typeof(IContainer) || typeof(T) == typeof(Container))
		    {
		        return (T)(object)new Container(this, name);
		    }

            var key = new Key(typeof(TState), typeof(T), name);
            object factory;
			if (_factories.TryGetValue(key, out factory))
			{
				return ((Func<TState, T>)factory)(state);				
			}

		    Exception innerException = null;
		    try
		    {
		        if (_parentContainer != null)
		        {
		            return _parentContainer.Resolve<TState, T>(state, name);
		        }
		    }
		    catch (InvalidOperationException ex)
		    {
		        innerException = ex;                
            }

            throw new InvalidOperationException($"The entry {key} was not registered. {GetRegisteredInfo()}", innerException);
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
