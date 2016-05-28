namespace DevTeam.Patterns.IoC.Tests
{
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

		private static Container CreateTarget(string name = "")
		{
			return new Container();
		}
	}
}
