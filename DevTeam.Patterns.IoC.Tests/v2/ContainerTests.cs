namespace DevTeam.Patterns.IoC.Tests.v2
{
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class ContainerTests
    {
        [Test]
        [TestCase(null)]
        [TestCase(1)]
        [TestCase("abc")]
        public void ShouldGetKeyWhenKeyWasDefined(object key)
        {
            // Given

            // When
            var target = CreateTarget(key);

            // Then
            target.Key.ShouldBe(key);
        }

        private static DevTeam.Patterns.IoC.v2.IContainer CreateTarget(object key = null)
        {
            return new DevTeam.Patterns.IoC.v2.Container(key);
        }
    }
}
