namespace DevTeam.Patterns.IoC
{
    using System.Collections.Generic;
    using System.Reflection;

    internal class LowerAccuracy : IAccuracy
    {
        public IEnumerable<IKey> GetResolverKeys(KeyDescription keyDescription)
        {
            yield return new StrictKey(keyDescription);
            yield return new GenericKey(keyDescription);
            foreach (var implementedInterface in keyDescription.InstanceType.GetTypeInfo().ImplementedInterfaces)
            {
                yield return new StrictKey(new KeyDescription(keyDescription.StateType, implementedInterface, keyDescription.Name, keyDescription.Resources));
            }
        }
    }
}