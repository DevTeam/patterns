namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;    

    internal class JsonConfiguration: IConfiguration
    {
        private readonly ConfigurationElement _configurationElement;

        public JsonConfiguration(
            string configuration)
        {            
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
            _configurationElement = JsonConvert.DeserializeObject<ConfigurationElement>(configuration);            
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            return 
                from dependency in _configurationElement.Dependencies ?? Enumerable.Empty<DependencyElement>()
                let dependencyType = Type.GetType(dependency.Type, true)
                select (IConfiguration)Activator.CreateInstance(dependencyType);
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return ApplyContainer(container, _configurationElement.Containers, _configurationElement.Scopes);
        }

        private IEnumerable<IRegistration> ApplyContainer(IContainer container, IEnumerable<ContainerElement> children, IEnumerable<ScopeElement> scopes)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return (
                from scope in scopes ?? Enumerable.Empty<ScopeElement>()
                select ApplyScope(container, scope)).Concat(
                from childContrainerElement in children ?? Enumerable.Empty<ContainerElement>()
                let childContainer = container.Resolve<IContainer>(GetKey(childContrainerElement.Key))
                select ApplyContainer(childContainer, childContrainerElement.Containers, childContrainerElement.Scopes)).SelectMany(i => i);
        }

        private IEnumerable<IRegistration> ApplyScope(IContainer container, ScopeElement scopeElement)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            return 
                from bindElement in scopeElement?.Binds ?? Enumerable.Empty<BindElement>()
                let stateType = bindElement.State != null ? Type.GetType(bindElement.State, true) : typeof(EmptyState)
                let instanceType = Type.GetType(bindElement.Instance, true)
                let implementationType = Type.GetType(bindElement.Implementation, true)
                let key = GetKey(bindElement.Key)
                let usingContainer = (
                    from usingElement in scopeElement?.Using ?? Enumerable.Empty<UsingElement>()
                    let usingType = Type.GetType(usingElement.Type, true)
                    let usingKey = GetKey(usingElement.Key)
                    select new { usingType, usingKey }).Aggregate(container, (usingContainer, usingInfo) => usingContainer.Using(usingInfo.usingType, usingInfo.usingKey))
                select usingContainer.Bind(stateType, instanceType, implementationType, key);
        }

        private object GetKey(KeyElement keyElement)
        {
            if (keyElement == null)
            {
                return null;
            }

            Type keyType;
            if (!string.IsNullOrWhiteSpace(keyElement.Type))
            {
                keyType = Type.GetType(keyElement.Type, true);
            }
            else
            {
                keyType = typeof(string);
            }

            return Convert.ChangeType(keyElement.Value, keyType);
        }
    }
}
