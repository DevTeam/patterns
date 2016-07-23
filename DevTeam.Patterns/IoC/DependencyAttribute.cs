namespace DevTeam.Patterns.IoC
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyAttribute: Attribute
    {
       public Type StateType { get; set; }

        public Type InstanceType { get; set; }

        public object Key { get; set; }

        public object State { get; set; }
    }
}