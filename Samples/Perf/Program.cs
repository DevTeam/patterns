namespace Perf
{
    using System;

    using DevTeam.Patterns.IoC;

    public class Program
    {
        public static void Main(string[] args)
        {
            var max = 1000000;
            var mod = 200;

            IContainer rootContainer = Containers.Create();
            var container = rootContainer;
            Console.WriteLine("Creates containers");
            for (var i = 0; i < 3; i++)
            {
                container = container.CreateChildContainer();
            }

            Console.WriteLine("Creates registrations");
            for (var i = 0; i < mod; i++)
            {
                rootContainer.Register<int, int>(n => n + 1, i);
                rootContainer.Register<MyClass>().As<int, MyClass>(i);
            }

            Console.WriteLine("Resolves");
            for (var i = 0; i < max; i++)
            {
                container.Resolve<int, int>(i, i % mod);
                container.Resolve<int, MyClass>(i, i % mod);
                var percent = 100.0 * i / max;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (percent % 10 == 0)
                {
                    Console.WriteLine($"{percent} %");
                }
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
