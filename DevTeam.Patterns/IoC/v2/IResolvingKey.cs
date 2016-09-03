namespace DevTeam.Patterns.IoC.v2
{
    using System;
    using System.Collections.Generic;

    public interface IResolvingKey: IEquatable<IResolvingKey>, IEnumerable<IKey>
    {
    }
}
