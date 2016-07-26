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
        [TestCase(typeof(string), typeof(IService), "abc", typeof(string), typeof(IService), "abc", true)]
        [TestCase(typeof(string), typeof(IService), "abc1", typeof(string), typeof(IService), "abc2", false)]
        [TestCase(typeof(int), typeof(IService), "abc", typeof(string), typeof(IService), "abc", false)]
        [TestCase(typeof(string), typeof(Service1), "abc", typeof(string), typeof(IService), "abc", false)]
        [TestCase(typeof(string), typeof(IService2<>), "abc", typeof(string), typeof(IService2<>), "abc", true)]
        public void ShouldSupportEquAndHash(
            Type stateType1, Type contractType1, string name1, 
            Type stateType2, Type contractType2, string name2, bool expectedEqu)
		{
			// Given
			var key1 = CreateTarget(stateType1, contractType1, name1);
            var key2 = CreateTarget(stateType2, contractType2, name2);

            // When

            // Then
            (key1.GetHashCode() == key2.GetHashCode()).ShouldBe(expectedEqu);
            Equals(key1, key2).ShouldBe(expectedEqu);
        }

        private static StrictRegistration CreateTarget(Type stateType, Type contractType, string name, IDisposable resources)
        {
			return new StrictRegistration(new RegistrationDescription(stateType, contractType, name, resources));
		}

        private static StrictRegistration CreateTarget(Type stateType, Type contractType, string name)
        {
            return CreateTarget(stateType, contractType, name, Disposable.Empty());
        }
    }
}
