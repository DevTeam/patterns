namespace DevTeam.Patterns.IoC
{
    using System;    

    public class Transient : IFactory
    {
    
        public object Create(Func<object, object> factory, object state)
        {
            return factory(state);
        }
    }
}
