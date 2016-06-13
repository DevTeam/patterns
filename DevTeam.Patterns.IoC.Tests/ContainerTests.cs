namespace DevTeam.Patterns.IoC.Tests
{
	using System.Linq;
	using Moq;

	using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class ContainerTests
	{
		private Mock<IService1> _service1;
		private Service1State _service1State;

		[SetUp]
		public void SetUp()
		{
			_service1 = new Mock<IService1>();
			_service1State = new Service1State();
		}

		[Test]
		public void ShouldRegister()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");

			// Then
			target.Registrations.ShouldContain(i => i.StateType == typeof(Service1State) && i.InstanceType == typeof(IService1) && i.Name == "myService1");				
		}

		[Test]
		public void ShouldRemoveRegistrationWhenRegistrationTokenIsDisposed()
		{
			// Given
			var target = CreateTarget();
			var registrationToken = target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");

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
			target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");
			
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
			target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");

			// When
			var instance = target.Resolve(typeof(Service1State), typeof(IService1), new Service1State(), "myService1");

			// Then
			instance.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveFromChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");

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
			target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");

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
			target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");

			// When
			var instances = target.Resolve(key => true, key => new Service1State()).ToList();

			// Then
			instances.Count.ShouldBe(1);
			instances[0].Item1.StateType.ShouldBe(typeof(Service1State));
			instances[0].Item1.InstanceType.ShouldBe(typeof(IService1));
			instances[0].Item1.Name.ShouldBe("myService1");
			instances[0].Item2.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveMultFromChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new");
			var instances = childContainer.Resolve(key => true, key => new Service1State()).ToList();

			// Then
			instances.Count.ShouldBe(1);
			instances[0].Item1.StateType.ShouldBe(typeof(Service1State));
			instances[0].Item1.InstanceType.ShouldBe(typeof(IService1));
			instances[0].Item1.Name.ShouldBe("myService1");
			instances[0].Item2.ShouldBe(_service1.Object);
		}

		[Test]
		public void ShouldResolveMultFromMultChildContainer()
		{
			// Given
			var target = CreateTarget();
			target.Register(typeof(Service1State), typeof(IService1), state => _service1.Object, "myService1");

			// When
			var childContainer = target.Resolve<IContainer>("new").Resolve<IContainer>("new2");
			var instances = childContainer.Resolve(key => true, key => new Service1State()).ToList();

			// Then
			instances.Count.ShouldBe(1);
			instances[0].Item1.StateType.ShouldBe(typeof(Service1State));
			instances[0].Item1.InstanceType.ShouldBe(typeof(IService1));
			instances[0].Item1.Name.ShouldBe("myService1");
			instances[0].Item2.ShouldBe(_service1.Object);
		}

		private static Container CreateTarget(string name = "")
		{
			return new Container();
		}
	}
}
