using System.Collections.Generic;

namespace DevTeam.Patterns.IoC.Tests
{
    using System;
    using System.Linq;
	using Moq;

	using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class ContainerTests
	{
		private Mock<IService> _service1;
        private Mock<IService> _service2;
        private Service1State _service1State;
	    private Mock<IService2<int>> _service3;
        private Mock<IService2<string>> _service4;

        [SetUp]
		public void SetUp()
		{
			_service1 = new Mock<IService>();
            _service2 = new Mock<IService>();
            _service3 = new Mock<IService2<int>>();
            _service4 = new Mock<IService2<string>>();
            _service1State = new Service1State();
		}

		[Test]
		public void ShouldRegister()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");

			// Then
			target.Registrations.ShouldContain(i => i.StateType == typeof(Service1State) && i.ContractType == typeof(IService) && "myService1".Equals(i.Key));
		}

		[Test]
		public void ShouldRemoveRegistrationWhenRegistrationTokenIsDisposed()
		{
			// Given
			var target = CreateTarget();
			var registrationToken = target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");

			// When
			registrationToken.Dispose();

			// Then
			target.Registrations.ShouldNotContain(i => i.StateType == typeof(Service1State) && i.ContractType == typeof(IService) && "myService1".Equals(i.Key));
		}

		[Test]
		public void ShouldRegisterAndResolve()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");
			
			// Then
			var resolvedContract = target.Resolve(target, typeof(Service1State), typeof(IService), _service1State, "myService1");
			resolvedContract.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldCreateChildContainerViaKey()
		{
			// Given
			var target = CreateTarget();

			// When
			var childContainer = (IContainer)target.Resolve(target, typeof(EmptyState), typeof(IContainer), EmptyState.Shared, "child");

			// Then
			childContainer.ShouldBeAssignableTo<IContainer>();
            childContainer.Key.ShouldBe("child");
        }

        [Test]
		public void ChildContainerShouldHasNameFromResolve()
		{
			// Given
			var target = CreateTarget();

			// When
			var childContainer = (IContainer)target.Resolve(target, typeof(EmptyState), typeof(IContainer), EmptyState.Shared, "child123");

			// Then
			childContainer.Key.ShouldBe("child123");
		}

		[Test]
		public void ShouldResolve()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");

			// When
			var contract = target.Resolve(target, typeof(Service1State), typeof(IService), new Service1State(), "myService1");

			// Then
			contract.ShouldBe(_service1.Object);
		}

        [Test]
        public void ShouldResolveGeneric()
        {
            // Given
            var target = CreateTarget();
            target.Register(typeof(Service1State), typeof(IService2<>), ctx => _service3.Object, "myService2");

            // When
            var contract = target.Resolve(target, typeof(Service1State), typeof(IService2<int>), new Service1State(), "myService2");

            // Then
            contract.ShouldBe(_service3.Object);
        }

        [Test]
        public void ShouldResolveUndefinedGeneric()
        {
            // Given
            var target = CreateTarget();
            target.Register(typeof(Service1State), typeof(IService2<>), ctx => _service3.Object, "myService2");

            // When
            var contract = target.Resolve(target, typeof(Service1State), typeof(IService2<>), new Service1State(), "myService2");

            // Then
            contract.ShouldBe(_service3.Object);
        }

        public void ShouldResolveGenericWhenDefGeneric()
        {
            // Given
            var target = CreateTarget();
            target.Register(typeof(Service1State), typeof(IService2<int>), ctx => _service3.Object, "myService2");

            // When
            var contract = target.Resolve(target, typeof(Service1State), typeof(IService2<int>), new Service1State(), "myService2");

            // Then
            contract.ShouldBe(_service3.Object);
        }

        [Test]
		public void ShouldResolveFromChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new");
			var contract = childContainer.Resolve(childContainer, typeof(Service1State), typeof(IService), new Service1State(), "myService1");

			// Then
			contract.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveFromMultChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new").Resolve<IContainer>("new2");
			var contract = childContainer.Resolve(childContainer, typeof(Service1State), typeof(IService), new Service1State(), "myService1");

			// Then
			contract.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveMult()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");
            target.Register(typeof(Service1State), typeof(IService), ctx => _service2.Object, "myService2");

            // When
            var contracts = target.ResolveAll<Service1State, IService>(name => new Service1State()).ToList();

			// Then
			contracts.Count.ShouldBe(2);
			contracts.ShouldContain(_service1.Object);
            contracts.ShouldContain(_service2.Object);
        }

		[Test]
		public void ShouldResolveMultFromChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new");
            var contracts = childContainer.ResolveAll<Service1State, IService>(name => new Service1State()).ToList();
            
            // Then
            contracts.Count.ShouldBe(1);
			contracts[0].ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveMultFromMultChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new").Resolve<IContainer>("new2");
            var contracts = childContainer.ResolveAll<Service1State, IService>(name => new Service1State()).ToList();

            // Then
            contracts.Count.ShouldBe(1);
			contracts[0].ShouldBe(_service1.Object);
		}

        [Test]
        public void TypedResolverShouldBeSingletone()
        {
            // Given
            var target = CreateTarget();
            target.Register(() => 99);

            // When
            var resolver = target.Resolve<IResolver<int>>();
            var resolver2 = target.Resolve<IResolver<int>>();
            object resolver3 = target.Resolve<IResolver<string>>();

            // Then
            resolver.ShouldBe(resolver2);
            resolver.ShouldNotBe(resolver3);            
        }

        [Test]
        public void ShouldCreateTypedResolver()
        {
            // Given
            var target = CreateTarget();
            target.Register(() => 99);

            // When
            var resolver = target.Resolve<IResolver<int>>();            
            var val =  resolver.Resolve();

            // Then            
            val.ShouldBe(99);
        }

        [Test]
        public void TypedResolverWithStateShouldBeSingletone()
        {
            // Given
            var target = CreateTarget();
            target.Register<int, int>(i => i + 1);

            // When
            var resolver = target.Resolve<IResolver<int, int>>();
            var resolver2 = target.Resolve<IResolver<int, int>>();

            // Then            
            resolver.ShouldBe(resolver2);
        }

        [Test]
        public void ShouldCreateTypedResolverWithState()
        {
            // Given
            var target = CreateTarget();
            target.Register<int, int>(i => i + 1);

            // When
            var resolver = target.Resolve<IResolver<int, int>>();
            var val = resolver.Resolve(9);

            // Then            
            val.ShouldBe(10);
        }

        [Test]
        public void ShouldResolveGenerics()
        {
            // Given
            var target = CreateTarget();
            var service3 = new Service2Int();
            var service4 = new Service2String();
            target.Register(typeof(long), typeof(IService2<>), ctx => ctx.ResolvingContractType == typeof(IService2<int>) ? (object)service3 : service4, "abc");

            // When
            var server3 = target.Resolve<long, IService2<int>>(1, "abc");
            var server4 = target.Resolve<long, IService2<string>>(1, "abc");

            // Then
            server3.ShouldBe(service3);
            server4.ShouldBe(service4);
        }

        [Test]
        public void ShouldResolveGenericsWithDifStates()
        {
            // Given
            var target = CreateTarget();
            var service3 = new Service2Int();
            long sum = 0;
            target.Register(typeof(long), typeof(IService2<>), ctx => { sum+= (long)ctx.State; return service3; }, "abc");

            // When
            target.Resolve<long, IService2<int>>(3, "abc");
            target.Resolve<long, IService2<int>>(5, "abc");

            // Then
            sum.ShouldBe(8);
        }

        [Test]
        public void ShouldResolveGenericSingletones()
        {
            // Given
            var target = CreateTarget();
            var service3 = new Service2Int();
            var service4 = new Service2String();
            target.Using<ILifetime>(WellknownLifetime.Singleton).Register(typeof(long), typeof(IService2<>), ctx => ctx.ResolvingContractType == typeof(IService2<int>) ? (object)service3 : service4, "abc");

            // When
            var server3 = target.Resolve<long, IService2<int>>(1, "abc");
            var server4 = target.Resolve<long, IService2<string>>(1, "abc");

            // Then
            server3.ShouldBe(service3);
            server4.ShouldBe(service4);
        }

        [Test]
        public void ShouldResolveUsingCustomRegistryKeyComparer()
        {
            // Given
            var target = CreateTarget();
            var comparer = new Mock<IRegistrationComparer>();
            comparer.Setup(i => i.GetHashCode(It.IsAny<IRegistration>())).Returns<IRegistration>(key => key.GetHashCode());
            comparer.Setup(i => i.Equals(It.IsAny<IRegistration>(), It.IsAny<IRegistration>())).Returns<IRegistration, IRegistration>((key1, key2) => key1.Equals(key2));
            comparer.SetupGet(i => i.Key).Returns(WellknownRegistrationComparer.PatternKey);
            target.Using(comparer.Object, typeof(IRegistrationComparer)).Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "myService1");

            // When
            var contract = target.Resolve(target, typeof(Service1State), typeof(IService), new Service1State(), "myService1");

            // Then
            contract.ShouldBe(_service1.Object);
            comparer.Verify(i => i.GetHashCode(It.IsAny<IRegistration>()));
            comparer.Verify(i => i.Equals(It.IsAny<IRegistration>(), It.IsAny<IRegistration>()));
        }

        [Test]
        public void ShouldResolveUsingNamePattern()
        {
            // Given
            var target = CreateTarget();
            target.Using<IRegistrationComparer>(WellknownRegistrationComparer.PatternKey).Register(typeof(Service1State), typeof(IService), ctx => _service1.Object, "a+.");

            // When
            var contract = target.Resolve(target, typeof(Service1State), typeof(IService), new Service1State(), "abc");

            // Then
            contract.ShouldBe(_service1.Object);            
        }

        [Test]
        public void ShouldResolveAllContractsAsIEnumerableWhenKeyIsNull()
        {
            // Given
            var target = CreateTarget();
            var service1 = new Service1();
            var service2 = new Service1();
            var service3 = new Service1();
            var service4 = new Service1();
            target.Register(typeof(Service1State), typeof(IService), ctx => service1, "myService1");
            target.Register(typeof(Service1State), typeof(IService), ctx => service2);
            target.Register(typeof(Service1State), typeof(IService), ctx => service3, 10);
            target.Register(typeof(EmptyState), typeof(IService), ctx => service4, "myService4");

            // When
            var resolvedContract = ((IEnumerable<IService>)target.Resolve(target, typeof(Service1State), typeof(IEnumerable<IService>), _service1State, null)).ToList();

            // Then
            resolvedContract.ShouldBeSubsetOf(new [] { service1, service2, service3 });
        }

        [Test]
        public void ShouldProvideContextWhenResolve()
        {
            // Given
            var target = CreateTarget();
            var child = target.CreateChildContainer();
            var service = new Service2Int();
            target.Register(typeof(int), typeof(IService2<>),
                ctx =>
                {
                    ctx.ShouldNotBeNull();
                    ctx.Registration.Key.ShouldBe("abc");
                    ctx.Registration.ContractType.ShouldBe(typeof(IService2<>));
                    ctx.Registration.StateType.ShouldBe(typeof(int));
                    ctx.PerThreadResolvingId.ShouldNotBe(Guid.Empty);
                    ctx.ResolvingId.ShouldNotBe(Guid.Empty);
                    ctx.ResolvingContractType.ShouldBe(typeof(IService2<int>));
                    ctx.State.ShouldBe(1);
                    ctx.Resolver.ShouldBe(child);
                    return service;
                },
                "abc");

            // When
            child.Resolve<int, IService2<int>>(1, "abc");

            // Then            
        }

        [Test]
        public void ShouldProvideContextWhenResolveFromResolve()
        {
            // Given
            var target = CreateTarget();
            var child1 = target.CreateChildContainer("child1");
            var child2 = child1.CreateChildContainer("child2");
            var service = new Service1();
            child1.Register(typeof(string), typeof(IService), typeof(Service1WithStateAndDependency), "abc");
            target.Register(typeof(EmptyState), typeof(IService),
                ctx =>
                {
                    ctx.ShouldNotBeNull();
                    ctx.Registration.Key.ShouldBe("dep");
                    ctx.Registration.ContractType.ShouldBe(typeof(IService));
                    ctx.Registration.StateType.ShouldBe(typeof(EmptyState));
                    ctx.PerThreadResolvingId.ShouldNotBe(Guid.Empty);
                    ctx.ResolvingId.ShouldNotBe(Guid.Empty);
                    ctx.ResolvingContractType.ShouldBe(typeof(IService));
                    ctx.State.ShouldBe(EmptyState.Shared);
                    ctx.Resolver.ShouldBe(child2);
                    return service;
                },
                "dep");

            // When
            child2.Resolve<string, IService>("state", "abc");

            // Then            
        }

        [Test]
        public void ShouldResolveWhenPublicScopeForCurrentContainer()
        {
            // Given
            var target = CreateTarget();
            var child1 = target.CreateChildContainer();

            // When
            child1.Using<IScope>(WellknownScope.Public).Register<Service1State, IService>(ctx => _service1.Object, "myService1");

            // Then
            child1.Registrations.ShouldContain(i => i.StateType == typeof(Service1State) && i.ContractType == typeof(IService) && "myService1".Equals(i.Key));
            child1.Resolve<Service1State, IService>(new Service1State(), "myService1").ShouldBe(_service1.Object);
        }

        [Test]
        public void ShouldResolveWhenPublicScopeForOtherContainer()
        {
            // Given
            var target = CreateTarget();
            var child1 = target.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            // When
            child1.Using<IScope>(WellknownScope.Public).Register<Service1State, IService>(ctx => _service1.Object, "myService1");

            // Then
            child2.Registrations.ShouldContain(i => i.StateType == typeof(Service1State) && i.ContractType == typeof(IService) && "myService1".Equals(i.Key));
            child2.Resolve<Service1State, IService>(new Service1State(), "myService1").ShouldBe(_service1.Object);
        }

        [Test]
        public void ShouldResolveWhenInternalScopeForCurrentContainer()
        {
            // Given
            var target = CreateTarget();
            var child1 = target.CreateChildContainer();

            // When
            child1.Using<IScope>(WellknownScope.Internal).Register<Service1State, IService>(ctx => _service1.Object, "myService1");

            // Then
            child1.Registrations.ShouldContain(i => i.StateType == typeof(Service1State) && i.ContractType == typeof(IService) && "myService1".Equals(i.Key));
            child1.Resolve<Service1State, IService>(new Service1State(), "myService1").ShouldBe(_service1.Object);
        }

        [Test]
        public void ShouldResolveWhenInternalScopeForOtherContainer()
        {
            // Given
            var target = CreateTarget();
            var child1 = target.CreateChildContainer();
            var child2 = child1.CreateChildContainer();

            // When
            child1.Using<IScope>(WellknownScope.Internal).Register<Service1State, IService>(ctx => _service1.Object, "myService1");

            // Then
            child2.Registrations.ShouldNotContain(i => i.StateType == typeof(Service1State) && i.ContractType == typeof(IService) && "myService1".Equals(i.Key));
            try
            {
                child2.Resolve<Service1State, IService>(new Service1State(), "myService1");
                Assert.Fail("Expected exception is InvalidOperationException");
            }
            catch (InvalidOperationException)
            {                
            }
        }

        private static Container CreateTarget(object name = null)
		{
			return new Container();
		}       
    }
}
