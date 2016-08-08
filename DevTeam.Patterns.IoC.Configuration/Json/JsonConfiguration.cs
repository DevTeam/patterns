namespace DevTeam.Patterns.IoC.Configuration.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;    

    internal class JsonConfiguration: IConfiguration
    {
        private static readonly Regex VarRegex = new Regex(@"^\s*(?<name>.+)\s*=\s*(?<value>.+)\s*$", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        private readonly ConfigurationElement _configurationElement;        

        public JsonConfiguration(
            string configuration)
        {            
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            
            _configurationElement = JsonConvert.DeserializeObject<ConfigurationElement>(configuration);            
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            var vars = new Dictionary<string, string>();
            var newVars = CreateVars(_configurationElement, vars);
            return GetDependencies(_configurationElement, newVars);
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            var vars = new Dictionary<string, string>();
            return CreateConfigurationRegistrations(container, _configurationElement, false, vars);
        }

        private static IEnumerable<IConfiguration> GetDependencies(ConfigurationElement configurationElement, IDictionary<string, string> vars)
        {
            return
                from dependency in configurationElement.Dependencies ?? Enumerable.Empty<DependencyElement>()
                let dependencyType = Type.GetType(ResolverString(dependency.Type, vars), true)
                select (IConfiguration)Activator.CreateInstance(dependencyType);
        }

        private static IEnumerable<IRegistration> CreateConfigurationRegistrations(IContainer container, ConfigurationElement configurationElement, bool includeDependencies, IDictionary<string, string> vars)
        {
            var newVars = CreateVars(configurationElement, vars);

            var dependeciesRegistrations = includeDependencies ? (
                from dependency in GetDependencies(configurationElement, newVars)
                select dependency.CreateRegistrations(container)).SelectMany(i => i)
                : Enumerable.Empty<IRegistration>();

            var registrations = CreateRegistrations(container, configurationElement, newVars);

            var childrenRegistrations = (
                from contrainerElement in configurationElement.Containers?? Enumerable.Empty<ContainerElement>()
                let childCoontainer = container.Resolve<IContainer>(GetKey(contrainerElement.Key))
                select CreateConfigurationRegistrations(childCoontainer, contrainerElement, true, newVars)).SelectMany(i => i);

            return dependeciesRegistrations.Concat(registrations).Concat(childrenRegistrations);

        }

        private static Dictionary<string, string> CreateVars(ConfigurationElement configurationElement, IDictionary<string, string> vars)
        {
            var newVars = new Dictionary<string, string>(vars);

            var overridedVarsMatches = (
                from varElement in configurationElement.Vars ?? Enumerable.Empty<string>()
                select VarRegex.Match(varElement)).ToList();

            var overridedVars = 
                from match in overridedVarsMatches
                where match.Success && match.Groups.Count == 3
                select new { name = match.Groups[1].Value, value = match.Groups[2].Value };

            foreach (var overridedVar in overridedVars)
            {
                newVars[overridedVar.name] = overridedVar.value;
            }

            return newVars;
        }

        private static IEnumerable<IRegistration> CreateRegistrations(IContainer container, ConfigurationElement configurationElement, IDictionary<string, string> vars)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            return 
                from registrationElement in configurationElement?.Registrations ?? Enumerable.Empty<RegistrationElement>()
                let stateType = registrationElement.State != null ? Type.GetType(ResolverString(registrationElement.State, vars), true) : typeof(EmptyState)
                let contractType = Type.GetType(ResolverString(registrationElement.Contract, vars), true)
                let implementationType = Type.GetType(ResolverString(registrationElement.Implementation, vars), true)
                let key = GetKey(registrationElement.Key)
                select ApplyUsingContainer(container, registrationElement).Register(stateType, contractType, implementationType, key);
        }

        private static string ResolverString(string stringTorResolve, IDictionary<string, string> vars)
        {
            if (stringTorResolve == null)
            {
                return null;
            }

            foreach (var varElement in vars)
            {
                stringTorResolve = stringTorResolve.Replace($"$({varElement.Key})", varElement.Value);
            }

            return stringTorResolve;
        }

        private static IContainer ApplyUsingContainer(IContainer container, RegistrationElement registrationElement)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (registrationElement == null) throw new ArgumentNullException(nameof(registrationElement));

            if (registrationElement.Lifetime != null && registrationElement.Lifetime != WellknownLifetime.Transient)
            {
                container = container.Using<ILifetime>(registrationElement.Lifetime.Value);
            }

            if (registrationElement.RegistrationComparer != null && registrationElement.RegistrationComparer == WellknownRegistrationComparer.FullCompliance)
            {
                container = container.Using<IRegistrationComparer>(registrationElement.RegistrationComparer.Value);
            }

            if (registrationElement.Scope != null && registrationElement.Scope != WellknownScope.Public)
            {
                container = container.Using<IScope>(registrationElement.Scope);
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
