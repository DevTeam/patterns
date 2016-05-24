namespace DevTeam.Patterns.Reactive
{
    public static class WellknownSchedulers
    {
        public static readonly string SharedSingleThread = "SharedSingleThread";
        public static readonly string PrivateSingleThread = "PrivateSingleThread";
        public static readonly string SharedMultiThread = "SharedMultiThread";
        public static readonly string PrivateMultiThread = "PrivateMultiThread";
    }
}
