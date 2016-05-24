namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

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
        		
		public IRegistry Register<T>(Func<T> factory, string name = "")
		{
		    if (factory == null) throw new ArgumentNullException(nameof(factory));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    return Register(new Func<EmptyArg, T>(ignoredArg => factory()), name);
		}
		
		public IRegistry Register<TArg, T>(Func<TArg, T> factory, string name = "")
		{
		    if (factory == null) throw new ArgumentNullException(nameof(factory));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    var key = new Key(typeof(TArg), typeof(T), name);            
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

		public T Resolve<T>(string name = "")
		{
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    var service = Resolve<EmptyArg, T>(EmptyArg.Shared, name);			
			return service;
		}

		public T Resolve<TArg, T>(TArg arg, string name = "")
		{
		    if (arg == null) throw new ArgumentNullException(nameof(arg));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    if (typeof(T) == typeof(IContainer) || typeof(T) == typeof(Container))
		    {
		        return (T)(object)new Container(this, name);
		    }

            var key = new Key(typeof(TArg), typeof(T), name);
            object factory;
			if (_factories.TryGetValue(key, out factory))
			{
				return ((Func<TArg, T>)factory)(arg);				
			}

		    Exception innerException = null;
		    try
		    {
		        if (_parentContainer != null)
		        {
		            return _parentContainer.Resolve<TArg, T>(arg, name);
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

        private class EmptyArg
		{
			public static readonly EmptyArg Shared = new EmptyArg();

			private EmptyArg()
			{
			}
		}

        private class Key
        {
            private readonly Type _t;
            private readonly Type _arg;            
            private readonly string _name;

            public Key(Type arg, Type t, string name)
            {
                if (arg == null) throw new ArgumentNullException(nameof(arg));
                if (t == null) throw new ArgumentNullException(nameof(t));
                if (name == null) throw new ArgumentNullException(nameof(name));

                _t = t;
                _arg = arg;                
                _name = name;                
            }

            public override string ToString()
            {
                return $"{_t.FullName}({_arg.FullName}, \"{_name}\")";
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
                    var hashCode = _t.GetHashCode();
                    hashCode = (hashCode * 397) ^ _arg.GetHashCode();
                    hashCode = (hashCode * 397) ^ _name.GetHashCode();
                    return hashCode;
                }
            }

            private bool Equals(Key other)
            {
                return _t == other._t && _arg == other._arg && string.Equals(_name, other._name);
            }
        }
	}
}
