namespace DevTeam.Patterns.IoC
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyAttribute: Attribute
    {
       public Type StateType { get; set; }

        public Type ContractType { get; set; }

        public object Key { get; set; }

        public object State { get; set; }
    }
}