namespace DevTeam.Patterns.IoC.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class AutowiringContainerIntegrationTests
    {
        [Test]
        public void ShouldMakeRegistration()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register<Service1WithState>().As<string, IService>("myService1");

            // Then
            target.GetRegistrations().ShouldContain(i => i.StateType == typeof(string) && i.ContractType == typeof(IService) && "myService1".Equals(i.Key));
        }

        [Test]
        public void ShouldResolveWhenCtorHasNoParams()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register<Service1>().As<IService>("myService1");

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
            target.Register<Service1WithState>().As<string, IService>("myService1");

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
            target.Register<Service1>().As<IService>("dep");
            target.Register<Service1WithStateAndDependency>().As<string, IService>("myService1");

            // Then
            var contract = target.Resolve<string, IService>("state", "myService1");
            contract.ShouldBeOfType<Service1WithStateAndDependency>();
            contract.State.ShouldBe("state");
            contract.Dependency.ShouldBeOfType<Service1>();
        }

        [Test]
        public void ShouldResolveWhenCtorHasStateAndNotMarkedDependency()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register(() => 99);
            target.Register<Service1WithStateAndNotMarkedDependency>().As<string, IService>("myService1");

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
            target.Register<Service1>().As<IService>("dep");
            target.Register<Service1WithStateAndDependency>().As<string, IService>("dep2");
            target.Register<Service1WithStateAndDependencyFromAttr>().As<int, IService>("myService1");

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
            target.Register<Service1>().As<IService>("dep");
            target.Register<Service1WithStateAndDependency>().As<string, IService>("dep2");
            target.Register<Service1WithStateAndDependencyViaResolver>().As<int, IService>("myService1");

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
            target.Register<Service1WithStateAndEnumerableDependency>(WellknownLifetime.Singleton).As<int, Service1WithStateAndEnumerableDependency>("myService1");
            target.Register<Service1>().As<IService>("dep");
            target.Register<Service1>().As<IService>("dep2");

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
            target.Register<Service1WithStateAndEnumerableDependency>(WellknownLifetime.Singleton).As<int, Service1WithStateAndEnumerableDependency>("myService1");
            target.Register<Service1>().As<IService>("dep");
            target.Register<Service1>().As<IService>("dep2");

            // Then
            var contract = target.Resolve<int, Service1WithStateAndEnumerableDependency>(33, "myService1");
            contract.ShouldBeOfType<Service1WithStateAndEnumerableDependency>();
            contract.State.ShouldBe(33);
            var dpendencies = (IEnumerable<IService>)contract.Dependency;
            dpendencies.Count().ShouldBe(2);
        }

        [Test]
        public void ShouldResolveWhenGeneric()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register(typeof(Service2<>)).As(typeof(IService2<>), "myService1");

            // Then
            var contract = target.Resolve<IService2<int>>("myService1");
            contract.ShouldBeOfType<Service2<int>>();
        }

        [Test]
        public void ShouldResolveWhenDefinedGeneric()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register(typeof(Service2<int>)).As(typeof(IService2<int>), "myService1");

            // Then
            var contract = target.Resolve<IService2<int>>("myService1");
            contract.ShouldBeOfType<Service2<int>>();
        }

        [Test]
        public void ShouldResolveWhenGenericWithState()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register(typeof(Service2WithState<>)).As(typeof(int), typeof(IService2<>), "myService1");

            // Then
            var contract = target.Resolve<int, IService2<string>>(3, "myService1");
            contract.ShouldBeOfType<Service2WithState<string>>();
        }

        [Test]
        public void ShouldResolveWhenDefinedGenericWithState()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register(typeof(Service2WithState<DateTime>)).As(typeof(int), typeof(IService2<DateTime>), "myService1");

            // Then
            var contract = target.Resolve<int, IService2<DateTime>>(3, "myService1");
            contract.ShouldBeOfType<Service2WithState<DateTime>>();
        }

        [Test]
        public void ShouldResolveAllInheritanceWhenUseImplementigWithWellknownContractRange()
        {
            // Given
            var target = CreateTarget();
            
            // When
            target.Register<Service1>().InRange(WellknownContractRange.Implementation).As<IService3>().As("myService1");

            // Then
            target.Resolve<IService>("myService1");
            target.Resolve<IService3>("myService1");
            target.Resolve<Service1>("myService1");
        }

        [Test]
        public void ShouldResolveAllInheritanceWhenUseImplementig()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Register<Service1>().As<IService>().As<IService3>().As("myService1");

            // Then
            target.Resolve<IService>("myService1");
            target.Resolve<IService3>("myService1");
            target.Resolve<Service1>("myService1");
        }

        private static Container CreateTarget(object key = null)
        {
            return new Container(key);
        }
    }
}
