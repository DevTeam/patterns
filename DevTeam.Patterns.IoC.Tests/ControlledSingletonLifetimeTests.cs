namespace DevTeam.Patterns.IoC.Tests
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using NUnit.Framework;

    public class ControlledSingletonLifetimeTests : LifetimeTestsBase
    {
        [Test]
        [TestCase(typeof(int), typeof(string), "abc", typeof(string), typeof(long), typeof(DateTimeOffset), "xyz", typeof(DateTimeOffset), false)]
        [TestCase(typeof(int), typeof(string), "abc", typeof(string), typeof(int), typeof(string), "abc", typeof(string), true)]
        [TestCase(typeof(int), typeof(string), null, typeof(string), typeof(long), typeof(DateTimeOffset), null, typeof(DateTimeOffset), false)]
        [TestCase(typeof(int), typeof(string), null, typeof(string), typeof(int), typeof(string), null, typeof(string), true)]
        [TestCase(typeof(int), typeof(IList<>), "abc", typeof(IList<string>), typeof(int), typeof(IList<>), "abc", typeof(IList<DateTimeOffset>), false)]
        [TestCase(typeof(int), typeof(IList<>), "abc", typeof(IList<string>), typeof(int), typeof(IList<>), "abc", typeof(IList<string>), true)]
        public void ShouldCreateInstance(
            Type stateType1, Type contractType1, object key1, Type resolvingContractType1,
            Type stateType2, Type contractType2, object key2, Type resolvingContractType2,
            bool actualObjectsAreEqual)
        {
            // Given

            // When

            // Then
            ShouldCreateInstance(
                CreateTarget(),
                stateType1, contractType1, key1, resolvingContractType1,
                stateType2, contractType2, key2, resolvingContractType2,
                actualObjectsAreEqual);
        }

        [Test]
        public void ShouldDisposeWhenRelease()
        {
            // Given

            // When

            // Then
            ShouldDisposeWhenRelease(CreateTarget(), Times.Once());
        }

        private static ILifetime CreateTarget()
        {
            return new SingletonLifetime(new ControlledLifetime());
        }
    }
}
