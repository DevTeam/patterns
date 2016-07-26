namespace DevTeam.Patterns.IoC
{
    internal class PerContainerLifetime: KeyBasedLifetime
    {
        public PerContainerLifetime(ILifetime baseLifetime)
            :base(baseLifetime)
        {            
        }

        protected override object CreateKey(IResolvingContext ctx)
        {
            return new Key(ctx);
        }

        private class Key
        {
            private readonly IResolvingContext _ctx;
            
	        public Key(IResolvingContext ctx)
	        {
	            _ctx = ctx;	            
	        }
            
            public bool Equals(Key other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(_ctx.Resolver, other._ctx.Resolver) && Equals(_ctx.Registration, other._ctx.Registration) && _ctx.ResolvingContractType == other._ctx.ResolvingContractType && Equals(_ctx.State, other._ctx.State);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != GetType()) return false;
                return Equals((Key)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _ctx.Resolver.GetHashCode();
                    hashCode = (hashCode * 397) ^ _ctx.Registration.GetHashCode();
                    hashCode = (hashCode * 397) ^ _ctx.ResolvingContractType.GetHashCode();
                    hashCode = (hashCode * 397) ^ (_ctx.State?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }
        }
    }
}