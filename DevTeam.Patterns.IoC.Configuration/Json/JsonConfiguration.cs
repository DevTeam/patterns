﻿namespace DevTeam.Patterns.IoC.Configuration.Json
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
            return GetDependencies(_configurationElement);
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return CreateConfigurationRegistrations(container, _configurationElement, false);
        }

        private static IEnumerable<IConfiguration> GetDependencies(ConfigurationElement configurationElement)
        {
            return
                from dependency in configurationElement.Dependencies ?? Enumerable.Empty<DependencyElement>()
                let dependencyType = Type.GetType(dependency.Type, true)
                select (IConfiguration)Activator.CreateInstance(dependencyType);
        }

        private static IEnumerable<IRegistration> CreateConfigurationRegistrations(IContainer container, ConfigurationElement configurationElement, bool includeDependencies)
        {
            var dependeciesRegistrations = includeDependencies ? (
                from dependency in GetDependencies(configurationElement)
                select dependency.CreateRegistrations(container)).SelectMany(i => i)
                : Enumerable.Empty<IRegistration>();

            var bindingsRegistrations = CreateBindingsRegistrations(container, configurationElement);

            var childrenRegistrations = (
                from contrainerElement in configurationElement.Containers?? Enumerable.Empty<ContainerElement>()
                let childCoontainer = container.Resolve<IContainer>(GetKey(contrainerElement.Key))
                select CreateConfigurationRegistrations(childCoontainer, contrainerElement, true)).SelectMany(i => i);

            return dependeciesRegistrations.Concat(bindingsRegistrations).Concat(childrenRegistrations);

        }

        private static IEnumerable<IRegistration> CreateBindingsRegistrations(IContainer container, ConfigurationElement configurationElement)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return 
                from bindElement in configurationElement?.Bindings ?? Enumerable.Empty<BindElement>()
                let stateType = bindElement.State != null ? Type.GetType(bindElement.State, true) : typeof(EmptyState)
                let contractType = Type.GetType(bindElement.Contract, true)
                let implementationType = Type.GetType(bindElement.Implementation, true)
                let key = GetKey(bindElement.Key)
                select ApplyUsingContainer(container, bindElement).Bind(stateType, contractType, implementationType, key);
        }

        private static IContainer ApplyUsingContainer(IContainer container, BindElement bindElement)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (bindElement == null) throw new ArgumentNullException(nameof(bindElement));

            if (bindElement.Lifetime != null)
            {
                container = container.Using<ILifetime>(bindElement.Lifetime.Value);
            }

            if (bindElement.RegistrationComparer != null)
            {
                container = container.Using<IRegistrationComparer>(bindElement.RegistrationComparer.Value);
            }

            return container;
        }

        private static object GetKey(KeyElement keyElement)
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