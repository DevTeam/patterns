namespace DevTeam.Patterns.IoC.Tests
{
    using System;

    using Moq;

    using Shouldly;

    public class LifetimeTestsBase
    {
        public delegate void SetupResolvingContext(Mock<IResolvingContext> resolvingContext);

        public void ShouldCreateInstance(
            ILifetime target,
            Type stateType1, Type contractType1, object key1, Type resolvingContractType1,
            Type stateType2, Type contractType2, object key2, Type resolvingContractType2,
            bool actualObjectsAreEqual,
            SetupResolvingContext setupResolvingContext1 = null,
            SetupResolvingContext setupResolvingContext2 = null)
        {
            // Given
            IRegistration registration1;
            Mock<IResolvingContext> resolvingContext1;
            Mock<IReleasingContext> releasingContext1;
            CreateMocks(
                out registration1, out resolvingContext1, out releasingContext1,
                stateType1, contractType1, key1, resolvingContractType1);

            setupResolvingContext1?.Invoke(resolvingContext1);

            IRegistration registration2;
            Mock<IResolvingContext> resolvingContext2;
            Mock<IReleasingContext> releasingContext2;
            CreateMocks(
                out registration2, out resolvingContext2, out releasingContext2,
                stateType2, contractType2, key2, resolvingContractType2);

            setupResolvingContext2?.Invoke(resolvingContext2);

            var obj1 = new object();
            var obj2 = new object();

            // When
            var actualObject1 = target.Create(resolvingContext1.Object, ctx => obj1);
            var actualObject2 = target.Create(resolvingContext2.Object, ctx => obj2);
            var expectedObjectsAreEqual = Equals(actualObject1, actualObject2);

            // Then
            actualObjectsAreEqual.ShouldBe(expectedObjectsAreEqual);
        }

        public void ShouldDisposeWhenRelease(ILifetime target, Times times)
        {
            // Given
            IRegistration registration;
            Mock<IResolvingContext> resolvingContext;
            Mock<IReleasingContext> releasingContext;
            CreateMocks(
                out registration, out resolvingContext, out releasingContext,
                typeof(int), typeof(string), "abc", typeof(string));

            var disposable = new Mock<IDisposable>();
            
            // When
            var actualObject1 = target.Create(resolvingContext.Object, ctx => disposable.Object);
            target.Release(releasingContext.Object);

            // Then
            disposable.Verify(i => i.Dispose(), times);
        }

        private static void CreateMocks(
            out IRegistration registration, out Mock<IResolvingContext> resolvingContext, out Mock<IReleasingContext> releasingContext,
            Type stateType, Type contractType, object key, Type resolvingContractType)
        {
            registration = new Registration(stateType, contractType, key);

            resolvingContext = new Mock<IResolvingContext>();
            resolvingContext.SetupGet(i => i.Registration).Returns(registration);
            resolvingContext.SetupGet(i => i.ResolvingContractType).Returns(resolvingContractType);

            releasingContext = new Mock<IReleasingContext>();
            releasingContext.SetupGet(i => i.Registration).Returns(registration);
        }
    }
}
