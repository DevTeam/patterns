namespace DevTeam.TestTool.Engine.Contracts
{
    using System.Collections.Generic;

    public interface IAssembly
    {
        IEnumerable<IType> DefinedTypes { get; }

        IType GetType(string typeName);
    }
}
