namespace DevTeam.Patterns.Dispose.Tests
{
	using System;

	using Moq;

	using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class SerialDisposableTests
    {
		private Mock<IDisposable> _disposable1;
		private Mock<IDisposable> _disposable2;

		[SetUp]
		public void SetUp()
		{
			_disposable1 = new Mock<IDisposable>();
			_disposable2 = new Mock<IDisposable>();
		}

		[Test]
		public void ShouldAssignDisposableWhenCtor()
		{
            // Given

            // When
            var target = CreateTarget(_disposable1.Object);

            // Then
            target.Disposable.ShouldBe(_disposable1.Object);
		}

        [Test]
        public void ShouldAssignDisposableWhenSetter()
        {
            // Given
            var target = CreateTarget();

            // When   
            target.Disposable = _disposable1.Object;

            // Then
            target.Disposable.ShouldBe(_disposable1.Object);
        }

        [Test]
        public void ShouldHasDefaultDisposable()
        {
            // Given            

            // When   
            var target = CreateTarget();

            // Then
            target.Disposable.ShouldNotBeNull();
        }

        [Test]
        public void ShouldDisposePrevDisposableWhenChanged()
        {
            // Given
            var target = CreateTarget(_disposable1.Object);

            // When            
            target.Disposable = _disposable2.Object;

            // Then
            _disposable1.Verify(i => i.Dispose());
            target.Disposable.ShouldBe(_disposable2.Object);
        }

        [Test]
        public void ShouldDisposeAfterSetWhenDisposed()
        {
            // Given
            var target = CreateTarget();

            // When   
            target.Dispose();
            target.Disposable = _disposable1.Object;

            // Then
            _disposable1.Verify(i => i.Dispose());
            target.Disposable.ShouldBe(_disposable1.Object);
        }

        [Test]
        public void ShouldDisposWhenDisposed()
        {
            // Given
            var target = CreateTarget();

            // When   
            target.Disposable = _disposable1.Object;
            target.Dispose();

            // Then
            _disposable1.Verify(i => i.Dispose());
            target.Disposable.ShouldBe(_disposable1.Object);
        }

        private static SerialDisposable CreateTarget(IDisposable disposable)
		{
			return new SerialDisposable(disposable);
		}

        private static SerialDisposable CreateTarget()
        {
            return new SerialDisposable();
        }
    }
}
