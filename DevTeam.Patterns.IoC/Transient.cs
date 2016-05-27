namespace DevTeam.Patterns.IoC
{
    using System;    

    public class Transient : ILifetime
    {
    
        public object Create(Func<object, object> factory, object state)
        {
            var instance = factory(state);
            return instance;
        }
    }
}
