namespace DevTeam.Patterns.IoC.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class BindingsTests
	{
		[Test]
		public void ShouldMakeRegistration()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Bind<string, IService, Service1WithState>("myService1");

			// Then
			target.Registrations.ShouldContain(i => i.StateType == typeof(string) && i.InstanceType == typeof(IService) && "myService1".Equals(i.Key));
		}

        [Test]
        public void ShouldResolveWhenCtorHasNoParams()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Bind<IService, Service1>("myService1");

            // Then
            var instance = target.Resolve<IService>("myService1");
            instance.ShouldBeOfType<Service1>();            
        }

        [Test]
        public void ShouldResolveWhenCtorHasStateOnly()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Bind<string, IService, Service1WithState>("myService1");

            // Then
            var instance = target.Resolve<string, IService>("state", "myService1");
            instance.ShouldBeOfType<Service1WithState>();
            instance.State.ShouldBe("state");
        }

        [Test]
        public void ShouldResolveWhenCtorHasStateAndDependency()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Bind<IService, Service1>("dep");
            target.Bind<string, IService, Service1WithStateAndDependency>("myService1");

            // Then
            var instance = target.Resolve<string, IService>("state", "myService1");
            instance.ShouldBeOfType<Service1WithStateAndDependency>();
            instance.State.ShouldBe("state");
            instance.Dependency.ShouldBeOfType<Service1>();
        }


        [Test]
        public void ShouldResolveWhenCtorHasStateAndDependencyWithStateFromDependencyAttr()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Bind<IService, Service1>("dep");
            target.Bind<string, IService, Service1WithStateAndDependency>("dep2");
            target.Bind<int, IService, Service1WithStateAndDependencyFromAttr>("myService1");

            // Then
            var instance = target.Resolve<int, IService>(33, "myService1");
            instance.ShouldBeOfType<Service1WithStateAndDependencyFromAttr>();
            instance.State.ShouldBe(33);
            instance.Dependency.ShouldBeOfType<Service1WithStateAndDependency>();
        }

        [Test]
        public void ShouldResolveWhenCtorHasStateAndDependencyWithStateViaResolver()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Bind<IService, Service1>("dep");
            target.Bind<string, IService, Service1WithStateAndDependency>("dep2");
            target.Bind<int, IService, Service1WithStateAndDependencyViaResolver>("myService1");

            // Then
            var instance = target.Resolve<int, IService>(33, "myService1");
            instance.ShouldBeOfType<Service1WithStateAndDependencyViaResolver>();
            instance.State.ShouldBe(33);
            instance.Dependency.ShouldBeOfType<Service1WithStateAndDependency>();
        }

        [Test]
        public void ShouldResolveWhenCtorbHasStateAndEnumerableDependency()
        {
            // Given
            var target = CreateTarget();
            
            // When
            target.Using<ILifetime>(WellknownLifetime.Singleton).Bind<int, Service1WithStateAndEnumerableDependency, Service1WithStateAndEnumerableDependency>("myService1");
            target.Bind<IService, Service1>("dep");
            target.Bind<IService, Service1>("dep2");

            // Then
            var instance = target.Resolve<int, Service1WithStateAndEnumerableDependency>(33, "myService1");
            instance.ShouldBeOfType<Service1WithStateAndEnumerableDependency>();
            instance.State.ShouldBe(33);
            var dpendencies = (IEnumerable<IService>)instance.Dependency;
            dpendencies.Count().ShouldBe(2);
        }

        [Test]
        public void ShouldResolveWhenCtorbHasStateAndEnumerableDependencyAndChildContainer()
        {
            // Given
            var target = CreateTarget().Resolve<IContainer>("child");

            // When
            target.Using<ILifetime>(WellknownLifetime.Singleton).Bind<int, Service1WithStateAndEnumerableDependency, Service1WithStateAndEnumerableDependency>("myService1");
            target.Bind<IService, Service1>("dep");
            target.Bind<IService, Service1>("dep2");

            // Then
            var instance = target.Resolve<int, Service1WithStateAndEnumerableDependency>(33, "myService1");
            instance.ShouldBeOfType<Service1WithStateAndEnumerableDependency>();
            instance.State.ShouldBe(33);
            var dpendencies = (IEnumerable<IService>)instance.Dependency;
            dpendencies.Count().ShouldBe(2);
        }

        private static Container CreateTarget(object key = null)
		{
			return new Container(key);
		}          
    }
}
