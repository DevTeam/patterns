namespace DevTeam.Patterns.IoC.Tests
{
    using System;

    using Moq;

    using NUnit.Framework;

    using Shouldly;

    public class FullComplianceRegistrationComparerTests
    {
        [Test]
        public void ShouldReturnValidKey()
        {
            // Given
            var target = CreateTarget();

            // When

            // Then
            target.Key.ShouldBe(WellknownComparer.FullCompliance);
        }

        [Test]
        [TestCase(typeof(int), typeof(string), "abc", typeof(int), typeof(string), "abc", true)]
        [TestCase(typeof(int), typeof(string), null, typeof(int), typeof(string), null, true)]
        [TestCase(typeof(long), typeof(DateTimeOffset), "xyz", typeof(int), typeof(string), "abc", false)]
        [TestCase(typeof(long), typeof(DateTimeOffset), null, typeof(int), typeof(string), null, false)]

        [TestCase(typeof(int), typeof(double), "abc", typeof(int), typeof(string), "abc", false)]
        [TestCase(typeof(long), typeof(string), "abc", typeof(int), typeof(string), "abc", false)]
        [TestCase(typeof(int), typeof(string), "abc", typeof(int), typeof(string), "xyz", false)]
        public void ShouldCompareRegistrations(Type stateType1, Type contractType1, object key1, Type stateType2, Type contractType2, object key2, bool expectedEqual)
        {
            // Given
            var target = CreateTarget();
            var registration1 = CreateRegistration(stateType1, contractType1, key1);
            var registration2 = CreateRegistration(stateType2, contractType2, key2);

            // When
            var actualEqual = target.Equals(registration1, registration2);
            
            // Then
            actualEqual.ShouldBe(expectedEqual);
        }

        private static IRegistration CreateRegistration(Type stateType, Type contractType, object key)
        {
            var registration = new Mock<IRegistration>();
            registration.SetupGet(i => i.StateType).Returns(stateType);
            registration.SetupGet(i => i.ContractType).Returns(contractType);
            registration.SetupGet(i => i.Key).Returns(key);
            registration.Setup(i => i.Equals(It.IsAny<IRegistration>())).Returns<IRegistration>(other => other.StateType == stateType && other.ContractType == contractType && Equals(other.Key, key));
            return registration.Object;
        }

        private static IComparer CreateTarget()
        {
            return new FullComplianceComparer();
        }
    }
}
