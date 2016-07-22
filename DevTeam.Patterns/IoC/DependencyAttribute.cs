namespace DevTeam.Patterns.IoC
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyAttribute: Attribute
    {
        public DependencyAttribute(Type stateType, Type instanceType, object key = null)
        {
            StateType = stateType ?? typeof(EmptyState);
            InstanceType = instanceType;
            Key = key;
        }

        public DependencyAttribute(Type instanceType, object key = null)
            :this(typeof(EmptyState), instanceType, key)
        {            
        }

        public DependencyAttribute(object key = null)
            : this(null, key)
        {
        }

        public Type StateType { get; }

        public Type InstanceType { get; }

        public object Key { get; }
    }
}