namespace DevTeam.Patterns.IoC
{
    using System.Threading.Tasks;

    public static class Async
    {
        public static async Task<T> Resolve<T>(this IResolver resolver, string name = "")            
        {
            return await resolver.Resolve<Task<T>>(name);
        }

        public static async Task<T> Resolve<TArg, T>(this IResolver resolver, TArg arg, string name = "")
        {
            return await resolver.Resolve<TArg, Task<T>>(arg, name);
        }
    }
}