﻿namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Dispose;

    public class Container: IContainer
	{
        private static readonly IConfiguration Configuration = new IoCContainerConfiguration();
        private readonly Dictionary<IRegestryKey, Func<IResolvingContext, object>> _factories = new Dictionary<IRegestryKey, Func<IResolvingContext, object>>(RootContainerRegestryKeyComparer.Shared);
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

	    public IEnumerable<IRegestryKey> Keys => _factories.Keys.Union(_parentContainer != null ? _parentContainer.Keys : Enumerable.Empty<IRegestryKey>());

	    public IDisposable Register(Type stateType, Type instanceType, Func<IResolvingContext, object> factoryMethod, string name = "")
		{
		    if (stateType == null) throw new ArgumentNullException(nameof(stateType));
		    if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
		    if (factoryMethod == null) throw new ArgumentNullException(nameof(factoryMethod));
		    if (name == null) throw new ArgumentNullException(nameof(name));

            var registration = new CompositeDisposable();
            var resources = new CompositeDisposable();
            var keyDescription = new KeyDescription(stateType, instanceType, name, resources);
	        var key = new StrictRegestryKey(keyDescription);
            try
	        {
	            if (instanceType != typeof(ILifetime))
	            {
	                var lifetime = (ILifetime)Resolve(typeof(EmptyState), typeof(ILifetime), EmptyState.Shared);
                    _factories.Add(key, ctx => lifetime.Create(ctx, factoryMethod));
                    resources.Add(Disposable.Create(() => Unregister(new ReleasingContext(this, key, name), lifetime)));
	            }
	            else
	            {
	                _factories.Add(key, factoryMethod);
	                resources.Add(Disposable.Create(() => Unregister(key)));
	            }

	            registration.Add(key);
	        }
	        catch (Exception ex)
	        {
	            throw new InvalidOperationException($"The entry {key} registration failed. Registered entries:\n{GetRegisteredInfo()}", ex);
	        }
	        
	        return registration;
		}

	    public object Resolve(Type stateType, Type instanceType, object state, string name = "")
        {
            if (stateType == null) throw new ArgumentNullException(nameof(stateType));
            if (instanceType == null) throw new ArgumentNullException(nameof(instanceType));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var keyDescription = new KeyDescription(stateType, instanceType, name, Disposable.Empty());
	        foreach (var key in GetResolverKeys(keyDescription))
	        {
	            Func<IResolvingContext, object> factory;
	            if (_factories.TryGetValue(key, out factory))
                {
                    return factory(new ResolvingContext(this, key, instanceType, state));
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

	        var keys = string.Join(" or ", GetResolverKeys(keyDescription).Select(i => i.ToString()));
            throw new InvalidOperationException($"The entries {keys} was not registered. {GetRegisteredInfo()}", innerException);
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

	    internal IEnumerable<IRegestryKey> Registrations => _factories.Keys;

	    private bool Unregister(IRegestryKey regestryKey)
        {
            return _factories.Remove(regestryKey);
        }

        private bool Unregister(IReleasingContext ctx, ILifetime factory)
        {
            if (Unregister(ctx.RegestryKey))
            {
                factory?.Release(ctx);
                return true;
            }

            return false;
        }

        private string GetRegisteredInfo()
	    {
	        var details = _factories.Count == 0 ? "no entries" : string.Join(", ", _factories.Keys.Select(k => k.ToString()));
            return $"Container \"{Name}\". Registered entries: {details}";
	    }

        private static IEnumerable<IRegestryKey> GetResolverKeys(KeyDescription keyDescription)
        {
            yield return new StrictRegestryKey(keyDescription);
            yield return new GenericRegestryKey(keyDescription);            
        }
	}
}
