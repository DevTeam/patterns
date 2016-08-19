namespace DevTeam.Patterns.IoC
{
    public enum WellknownComparer
    {
        /// <summary>
        /// Checks identity by state type, contract type and key.
        /// </summary>
        FullCompliance,

        /// <summary>
        ///Checks identity by state type, contract type and by a key which is converted to string using regular expression.
        /// </summary>
        PatternKey,

        /// <summary>
        /// Checks identity by state type, contract type.
        /// </summary>
        AnyKey,

        /// <summary>
        /// Checks identity by contract type only.
        /// </summary>
        AnyStateTypeAndKey
    }
}