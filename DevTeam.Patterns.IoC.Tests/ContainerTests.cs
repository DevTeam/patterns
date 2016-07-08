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
		private Mock<IService1> _service1;
        private Mock<IService1> _service2;
        private Service1State _service1State;
	    private Mock<IService2<int>> _service3;
        private Mock<IService2<string>> _service4;

        [SetUp]
		public void SetUp()
		{
			_service1 = new Mock<IService1>();
            _service2 = new Mock<IService1>();
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
			target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");

			// Then
			target.Registrations.ShouldContain(i => i.StateType == typeof(Service1State) && i.InstanceType == typeof(IService1) && "myService1".Equals(i.Key));
		}

		[Test]
		public void ShouldRemoveRegistrationWhenRegistrationTokenIsDisposed()
		{
			// Given
			var target = CreateTarget();
			var registrationToken = target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");

			// When
			registrationToken.Dispose();

			// Then
			target.Registrations.ShouldNotContain(i => i.StateType == typeof(Service1State) && i.InstanceType == typeof(IService1) && "myService1".Equals(i.Key));
		}

		[Test]
		public void ShouldRegisterAndResolve()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");
			
			// Then
			var resolvedInstance = target.Resolve(typeof(Service1State), typeof(IService1), _service1State, "myService1");
			resolvedInstance.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldCreateChildContainerViaKey()
		{
			// Given
			var target = CreateTarget();

			// When
			var childContainer = (IContainer)target.Resolve(typeof(EmptyState), typeof(IContainer), EmptyState.Shared, "child");

			// Then
			childContainer.ShouldBeAssignableTo<IContainer>();
            childContainer.Key.ShouldBe("child");
        }

        [Test]
        public void ShouldCreateChildContainerViaState()
        {
            // Given
            var target = CreateTarget();

            // When
            var childContainer = (IContainer)target.Resolve(typeof(object), typeof(IContainer), "child");

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
			var childContainer = (IContainer)target.Resolve(typeof(EmptyState), typeof(IContainer), EmptyState.Shared, "child123");

			// Then
			childContainer.Key.ShouldBe("child123");
		}

		[Test]
		public void ShouldResolve()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");

			// When
			var instance = target.Resolve(typeof(Service1State), typeof(IService1), new Service1State(), "myService1");

			// Then
			instance.ShouldBe(_service1.Object);
		}

        [Test]
        public void ShouldResolveGeneric()
        {
            // Given
            var target = CreateTarget();
            target.Register(typeof(Service1State), typeof(IService2<>), ctx => _service3.Object, "myService2");

            // When
            var instance = target.Resolve(typeof(Service1State), typeof(IService2<int>), new Service1State(), "myService2");

            // Then
            instance.ShouldBe(_service3.Object);
        }

        [Test]
        public void ShouldResolveUndefinedGeneric()
        {
            // Given
            var target = CreateTarget();
            target.Register(typeof(Service1State), typeof(IService2<>), ctx => _service3.Object, "myService2");

            // When
            var instance = target.Resolve(typeof(Service1State), typeof(IService2<>), new Service1State(), "myService2");

            // Then
            instance.ShouldBe(_service3.Object);
        }

        public void ShouldResolveGenericWhenDefGeneric()
        {
            // Given
            var target = CreateTarget();
            target.Register(typeof(Service1State), typeof(IService2<int>), ctx => _service3.Object, "myService2");

            // When
            var instance = target.Resolve(typeof(Service1State), typeof(IService2<int>), new Service1State(), "myService2");

            // Then
            instance.ShouldBe(_service3.Object);
        }

        [Test]
		public void ShouldResolveFromChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new");
			var instance = childContainer.Resolve(typeof(Service1State), typeof(IService1), new Service1State(), "myService1");

			// Then
			instance.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveFromMultChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new").Resolve<IContainer>("new2");
			var instance = childContainer.Resolve(typeof(Service1State), typeof(IService1), new Service1State(), "myService1");

			// Then
			instance.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveMult()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");
            target.Register(typeof(Service1State), typeof(IService1), ctx => _service2.Object, "myService2");

            // When
            var instances = target.ResolveAll<Service1State, IService1>(name => new Service1State()).ToList();

			// Then
			instances.Count.ShouldBe(2);
			instances.ShouldContain(_service1.Object);
            instances.ShouldContain(_service2.Object);
        }

		[Test]
		public void ShouldResolveMultFromChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new");
            var instances = childContainer.ResolveAll<Service1State, IService1>(name => new Service1State()).ToList();
            
            // Then
            instances.Count.ShouldBe(1);
			instances[0].ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveMultFromMultChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new").Resolve<IContainer>("new2");
            var instances = childContainer.ResolveAll<Service1State, IService1>(name => new Service1State()).ToList();

            // Then
            instances.Count.ShouldBe(1);
			instances[0].ShouldBe(_service1.Object);
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
            target.Register(typeof(long), typeof(IService2<>), ctx => ctx.ResolvingInstanceType == typeof(IService2<int>) ? (object)service3 : service4, "abc");

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
            target.Using<ILifetime>(WellknownLifetime.Singleton).Register(typeof(long), typeof(IService2<>), ctx => ctx.ResolvingInstanceType == typeof(IService2<int>) ? (object)service3 : service4, "abc");

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
            var childContainer = target.Using(() => comparer.Object).CreateChildContainer();
            childContainer.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "myService1");

            // When
            var instance = childContainer.Resolve(typeof(Service1State), typeof(IService1), new Service1State(), "myService1");

            // Then
            instance.ShouldBe(_service1.Object);
            comparer.Verify(i => i.GetHashCode(It.IsAny<IRegistration>()));
            comparer.Verify(i => i.Equals(It.IsAny<IRegistration>(), It.IsAny<IRegistration>()));
        }

        [Test]
        public void ShouldResolveUsingNamePattern()
        {
            // Given
            var target = CreateTarget();
            var childContainer = target.Using<IRegistrationComparer>(WellknownRegistrationComparer.Pattern).CreateChildContainer();
            childContainer.Register(typeof(Service1State), typeof(IService1), ctx => _service1.Object, "a+.");

            // When
            var instance = childContainer.Resolve(typeof(Service1State), typeof(IService1), new Service1State(), "abc");

            // Then
            instance.ShouldBe(_service1.Object);            
        }

        private static Container CreateTarget(object name = null)
		{
			return new Container();
		}

        private class Service2Int: IService2<int>
        {            
        }

        private class Service2String : IService2<string>
        {
        }
    }
}
