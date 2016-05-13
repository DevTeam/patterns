namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;

    internal class Container : IContainer
	{
		private readonly Dictionary<string, object> _factories = new Dictionary<string, object>();
		private readonly Container _parentContainer;

		public Container()
		{
			Register<IContainer>(() => new Container(this));
		}

		private Container(Container parentContainer)
			:this()
		{
		    if (parentContainer == null) throw new ArgumentNullException(nameof(parentContainer));

		    _parentContainer = parentContainer;
		}
        		
		public IRegistry Register<T>(Func<T> factory, string name = "")
		{
		    if (factory == null) throw new ArgumentNullException(nameof(factory));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    return Register(new Func<EmptyArg, T>(arg => factory()), name);
		}
		
		public IRegistry Register<TArg, T>(Func<TArg, T> factory, string name = "")
		{
		    if (factory == null) throw new ArgumentNullException(nameof(factory));
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    var key = CreateKey<TArg, T>(name);
			if (_factories.ContainsKey(key))
			{
				throw new InvalidOperationException($"The entry {key} was alredy registered");
			}

			_factories.Add(key, factory);
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

		    var key = CreateKey<TArg, T>(name);
			object factory;
			if (_factories.TryGetValue(key, out factory))
			{
				return ((Func<TArg, T>)factory)(arg);				
			}

			if (_parentContainer != null)
			{
				return _parentContainer.Resolve<TArg, T>(arg, name);
			}

			throw new InvalidOperationException($"The entry {key} was not registered");
		}
					
		private static string CreateKey<TArg, T>(string name)
		{
		    if (name == null) throw new ArgumentNullException(nameof(name));

		    return $"{typeof(T).FullName}.{typeof(TArg).FullName}.{name}";
		}

		private class EmptyArg
		{
			public static readonly EmptyArg Shared = new EmptyArg();

			private EmptyArg()
			{
			}
		}
	}
}
