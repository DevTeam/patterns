namespace Perf
{
    using DevTeam.Patterns.IoC;

    public class Program
    {
        public static void Main(string[] args)
        {
            var max = 100000;

            IContainer rootContainer = new Container();
            var container = rootContainer;
            for (var i = 0; i < 3; i++)
            {
                container = container.CreateChildContainer();
            }

            for (var i = 0; i < max; i++)
            {
                rootContainer.Register<int, int>(n => n + 1, i);
                rootContainer.Bind<int, MyClass, MyClass>(i);
            }

            for (var i = 0; i < max; i++)
            {
                container.Resolve<int, int>(i, i);
                container.Resolve<int, MyClass>(i, i);
            }
        }

        private class MyClass
        {
            public MyClass([State] int state)
            {
            }
        }
    }
}
