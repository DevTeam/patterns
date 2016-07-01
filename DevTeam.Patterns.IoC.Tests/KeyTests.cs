namespace DevTeam.Patterns.IoC.Tests
{
    using System;

    using Dispose;

    using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class KeyTests
	{
		[Test]
        [TestCase(false, typeof(string), typeof(IService1), "abc", true, typeof(string), typeof(IService1), "abc", true)]
        [TestCase(false, typeof(string), typeof(IService1), "abc1", true, typeof(string), typeof(IService1), "abc2", false)]
        [TestCase(false, typeof(int), typeof(IService1), "abc", true, typeof(string), typeof(IService1), "abc", false)]
        [TestCase(false, typeof(string), typeof(Service1), "abc", true, typeof(string), typeof(IService1), "abc", false)]
        [TestCase(false, typeof(string), typeof(IService2<>), "abc", true, typeof(string), typeof(IService2<>), "abc", true)]
        [TestCase(false, typeof(string), typeof(IService2<int>), "abc", true, typeof(string), typeof(IService2<string>), "abc", false)]
        [TestCase(false, typeof(string), typeof(IService2<>), "abc", true, typeof(string), typeof(IService2<string>), "abc", true)]
        [TestCase(true, typeof(string), typeof(IService2<>), "abc", false, typeof(string), typeof(IService2<string>), "abc", false)]
        public void ShouldSupportEquAndHash(
            bool resolve1, Type stateType1, Type instanceType1, string name1, 
            bool resolve2, Type stateType2, Type instanceType2, string name2, bool expectedEqu)
		{
			// Given
			var key1 = CreateTarget(resolve1, stateType1, instanceType1, name1);
            var key2 = CreateTarget(resolve2, stateType2, instanceType2, name2);

            // When

            // Then
            (key1.GetHashCode() == key2.GetHashCode()).ShouldBe(expectedEqu);
            Equals(key1, key2).ShouldBe(expectedEqu);
        }

        private static Key CreateTarget(bool resolve, Type stateType, Type instanceType, string name, IDisposable resources)
        {
			return new Key(resolve, stateType, instanceType, name, resources);
		}

        private static Key CreateTarget(bool resolve, Type stateType, Type instanceType, string name)
        {
            return CreateTarget(resolve, stateType, instanceType, name, Disposable.Empty());
        }
    }
}
