namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Threading.Tasks;

    public static class Resolvers
    {
        public static async Task<T> Resolve<T>(this IResolver resolver, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return await resolver.Resolve<Task<T>>(name);
        }

        public static async Task<T> Resolve<TArg, T>(this IResolver resolver, TArg arg, string name = "")
        {
            if (resolver == null) throw new ArgumentNullException(nameof(resolver));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return await resolver.Resolve<TArg, Task<T>>(arg, name);
        }
    }
}