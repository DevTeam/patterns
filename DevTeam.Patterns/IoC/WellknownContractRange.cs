namespace DevTeam.Patterns.IoC
{
    public enum WellknownContractRange
    {
        /// <summary>
        /// Checks full identity of contract.
        /// </summary>
        Contract,

        /// <summary>
        /// Checks full identity and interface implementations of contract.
        /// </summary>
        Implementation,

        /// <summary>
        /// Checks full identity, interface implementations and base types of contract.
        /// </summary>
        Inheritance
    }
}