﻿namespace DevTeam.Patterns.IoC.Tests
{
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

	    [SetUp]
		public void SetUp()
		{
			_service1 = new Mock<IService1>();
            _service2 = new Mock<IService1>();
            _service3 = new Mock<IService2<int>>();
            _service1State = new Service1State();
		}

		[Test]
		public void ShouldRegister()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");

			// Then
			target.Registrations.ShouldContain(i => i.StateType == typeof(Service1State) && i.InstanceType == typeof(IService1) && i.Name == "myService1");				
		}

		[Test]
		public void ShouldRemoveRegistrationWhenRegistrationTokenIsDisposed()
		{
			// Given
			var target = CreateTarget();
			var registrationToken = target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");

			// When
			registrationToken.Dispose();

			// Then
			target.Registrations.ShouldNotContain(i => i.StateType == typeof(Service1State) && i.InstanceType == typeof(IService1) && i.Name == "myService1");
		}

		[Test]
		public void ShouldRegisterAndResolve()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");
			
			// Then
			var resolvedInstance = target.Resolve(typeof(Service1State), typeof(IService1), _service1State, "myService1");
			resolvedInstance.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldCreateChildContainer()
		{
			// Given
			var target = CreateTarget();

			// When
			var childContainer = target.Resolve(typeof(EmptyState), typeof(IContainer), "child");

			// Then
			childContainer.ShouldBeAssignableTo<IContainer>();
		}

		[Test]
		public void ChildContainerShouldHasNameFromResolve()
		{
			// Given
			var target = CreateTarget();

			// When
			var childContainer = (IContainer)target.Resolve(typeof(EmptyState), typeof(IContainer), EmptyState.Shared, "child123");

			// Then
			childContainer.Name.ShouldBe("child123");
		}

		[Test]
		public void ShouldResolve()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");

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
            target.Register(typeof(Service1State), typeof(IService2<>), (type, state) => _service3.Object, "myService2");

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
			target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");

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
			target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");

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
			target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");
            target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service2.Object, "myService2");

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
			target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");

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
			target.Register(typeof(Service1State), typeof(IService1), (type, state) => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new").Resolve<IContainer>("new2");
            var instances = childContainer.ResolveAll<Service1State, IService1>(name => new Service1State()).ToList();

            // Then
            instances.Count.ShouldBe(1);
			instances[0].ShouldBe(_service1.Object);
		}

        [Test]
        public void ShouldTypedResolverShouldBeSingletone()
        {
            // Given
            var target = CreateTarget();
            target.Register(() => 99);

            // When
            var resolver = target.Resolve<IResolver<int>>();
            var resolver2 = target.Resolve<IResolver<int>>();            

            // Then
            resolver.ShouldBe(resolver2);            
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

        //[Test]
        //public void ShouldCreateTypedResolverWithState()
        //{
        //    // Given
        //    var target = CreateTarget();
        //    target.Register<int, int>(i => i + 1);

        //    // When
        //    var resolver = target.Resolve<IResolver<int, int>>();
        //    var val = resolver.Resolve(9);

        //    // Then            
        //    val.ShouldBe(10);
        //}

        private static Container CreateTarget(string name = "")
		{
			return new Container();
		}
	}
}
