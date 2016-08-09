namespace DevTeam.Patterns.IoC
{
    public enum WellknownScope
    {
        /// <summary>
        /// Registration will be visible in the current and children containers.
        /// </summary>
        Public,

        /// <summary>
        /// Registration will be visible in the current container only.
        /// </summary>
        Internal,

        /// <summary>
        /// Registration will be visible in whole hierarchy of containers.
        /// </summary>
        Global
    }
}
