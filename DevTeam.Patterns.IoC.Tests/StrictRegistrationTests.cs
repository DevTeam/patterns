namespace DevTeam.Patterns.IoC.Tests
{
    using System;

    using Dispose;

    using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class StrictRegistrationTests
	{
		[Test]
        [TestCase(typeof(string), typeof(IService1), "abc", typeof(string), typeof(IService1), "abc", true)]
        [TestCase(typeof(string), typeof(IService1), "abc1", typeof(string), typeof(IService1), "abc2", false)]
        [TestCase(typeof(int), typeof(IService1), "abc", typeof(string), typeof(IService1), "abc", false)]
        [TestCase(typeof(string), typeof(Service1), "abc", typeof(string), typeof(IService1), "abc", false)]
        [TestCase(typeof(string), typeof(IService2<>), "abc", typeof(string), typeof(IService2<>), "abc", true)]
        public void ShouldSupportEquAndHash(
            Type stateType1, Type instanceType1, string name1, 
            Type stateType2, Type instanceType2, string name2, bool expectedEqu)
		{
			// Given
			var key1 = CreateTarget(stateType1, instanceType1, name1);
            var key2 = CreateTarget(stateType2, instanceType2, name2);

            // When

            // Then
            (key1.GetHashCode() == key2.GetHashCode()).ShouldBe(expectedEqu);
            Equals(key1, key2).ShouldBe(expectedEqu);
        }

        private static StrictRegistration CreateTarget(Type stateType, Type instanceType, string name, IDisposable resources)
        {
			return new StrictRegistration(new RegistrationDescription(stateType, instanceType, name, resources));
		}

        private static StrictRegistration CreateTarget(Type stateType, Type instanceType, string name)
        {
            return CreateTarget(stateType, instanceType, name, Disposable.Empty());
        }
    }
}
