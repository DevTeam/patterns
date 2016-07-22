using System.Collections.Generic;

namespace DevTeam.Platform.Reflection
{    
    public interface IAssembly
    {
        IEnumerable<IType> DefinedTypes { get; }

        IType GetType(string typeName);
    }
}
