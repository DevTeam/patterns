namespace IoC.Tests
{
    using System;
    using System.Linq;

    using Contracts;

    using Moq;

    using NUnit.Framework;

    using Shouldly;

    public class IntegrationTests
    {
        [Test]
        public void ShouldRegister()
        {
            // Given
            var container = new Container();
            var service = Mock.Of<Mocks.ISimpleService>();

            // When
            IDisposable registrationToken;
            container.TryRegister(out registrationToken, Keys.Create("abc").Implementing<Mocks.ISimpleService>(), Factories.Create(ctx => service)).ShouldBeTrue();
            using (registrationToken)
            {
                var registrations = container.Registrations.ToList();

                // Then
                registrations.Count.ShouldBe(1);
            }
        }

        [Test]
        public void ShouldResolve()
        {
            // Given
            var container = new Container();
            var service = Mock.Of<Mocks.ISimpleService>();

            // When
            IDisposable registrationToken;
            container.TryRegister(out registrationToken, Keys.Create("abc").Implementing<Mocks.ISimpleService>(), Factories.Create(ctx => service));
            using (registrationToken)
            {
                // Then
                object value;
                container.TryResolve(out value, Keys.Create("abc").Implementing<Mocks.ISimpleService>()).ShouldBe(true);
                value.ShouldBe(service);
            }
        }
    }
}