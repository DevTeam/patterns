namespace DevTeam.Patterns.IoC.Tests
{
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
			target.Map<string, IService, Service1WithState>("myService1");

			// Then
			target.Registrations.ShouldContain(i => i.StateType == typeof(string) && i.InstanceType == typeof(IService) && "myService1".Equals(i.Key));
		}

        [Test]
        public void ShouldResolveWhenCtorHasNoParams()
        {
            // Given
            var target = CreateTarget();

            // When
            target.Map<IService, Service1>("myService1");

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
            target.Map<string, IService, Service1WithState>("myService1");

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
            target.Map<IService, Service1>("dep");
            target.Map<string, IService, Service1WithStateAndDependency>("myService1");

            // Then
            var instance = target.Resolve<string, IService>("state", "myService1");
            instance.ShouldBeOfType<Service1WithStateAndDependency>();
            instance.State.ShouldBe("state");
            instance.Dependency.ShouldBeOfType<Service1>();
        }

        private static Container CreateTarget(object key = null)
		{
			return new Container(key);
		}          
    }
}
