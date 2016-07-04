namespace Perf
{
    using DevTeam.Patterns.IoC;

    public class Program
    {
        public static void Main(string[] args)
        {
            var max = 1000000;

            IContainer rootContainer = new Container();
            var container = rootContainer;
            for (var i = 0; i < 10; i++)
            {
                container = container.CreateChildContainer();
            }

            for (var i = 0; i < max; i++)
            {
                rootContainer.Register<int, int>(n => n + 1, i.ToString());
            }

            for (var i = 0; i < max; i++)
            {
                container.Resolve<int, int>(i, i.ToString());
            }
        }
    }
}
