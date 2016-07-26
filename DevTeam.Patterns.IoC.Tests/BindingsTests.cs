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
			target.Registrations.ShouldContain(i => i.StateType == typeof(string) && i.ContractType == typeof(IService) && "myService1".Equals(i.Key));
		}

        [Test]
        public void ShouldResolveWhenCtorHasNoParams()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Bind<IService, Service1>("myService1");

            // Then
            var contract = target.Resolve<IService>("myService1");
            contract.ShouldBeOfType<Service1>();            
        }

        [Test]
        public void ShouldResolveWhenCtorHasStateOnly()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Bind<string, IService, Service1WithState>("myService1");

            // Then
            var contract = target.Resolve<string, IService>("state", "myService1");
            contract.ShouldBeOfType<Service1WithState>();
            contract.State.ShouldBe("state");
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
            var contract = target.Resolve<string, IService>("state", "myService1");
            contract.ShouldBeOfType<Service1WithStateAndDependency>();
            contract.State.ShouldBe("state");
            contract.Dependency.ShouldBeOfType<Service1>();
        }

        [Test]
        public void ShouldResolveWhenCtorHasStateAndNptMarkedDependency()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register(() => 99);
            target.Bind<string, IService, Service1WithStateAndNotMarkedDependency>("myService1");

            // Then
            var contract = target.Resolve<string, IService>("state", "myService1");
            contract.ShouldBeOfType<Service1WithStateAndNotMarkedDependency>();
            contract.State.ShouldBe("state");
            contract.Dependency.ShouldBe(99);
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
            var contract = target.Resolve<int, IService>(33, "myService1");
            contract.ShouldBeOfType<Service1WithStateAndDependencyFromAttr>();
            contract.State.ShouldBe(33);
            contract.Dependency.ShouldBeOfType<Service1WithStateAndDependency>();
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
            var contract = target.Resolve<int, IService>(33, "myService1");
            contract.ShouldBeOfType<Service1WithStateAndDependencyViaResolver>();
            contract.State.ShouldBe(33);
            contract.Dependency.ShouldBeOfType<Service1WithStateAndDependency>();
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
            var contract = target.Resolve<int, Service1WithStateAndEnumerableDependency>(33, "myService1");
            contract.ShouldBeOfType<Service1WithStateAndEnumerableDependency>();
            contract.State.ShouldBe(33);
            var dpendencies = (IEnumerable<IService>)contract.Dependency;
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
            var contract = target.Resolve<int, Service1WithStateAndEnumerableDependency>(33, "myService1");
            contract.ShouldBeOfType<Service1WithStateAndEnumerableDependency>();
            contract.State.ShouldBe(33);
            var dpendencies = (IEnumerable<IService>)contract.Dependency;
            dpendencies.Count().ShouldBe(2);
        }

        private static Container CreateTarget(object key = null)
		{
			return new Container(key);
		}          
    }
}
