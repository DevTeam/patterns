namespace DevTeam.Patterns.Dispose.Tests
{
	using System;

	using Moq;

	using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class SerialDisposableTests
    {
		private Mock<IDisposable> _disposable;
		private Mock<IDisposable> _disposable2;

		[SetUp]
		public void SetUp()
		{
			_disposable = new Mock<IDisposable>();
			_disposable2 = new Mock<IDisposable>();
		}

		[Test]
		public void ShouldAssignDisposableWhenCtor()
		{
            // Given

            // When
            var target = CreateTarget(_disposable.Object);

            // Then
            target.Disposable.ShouldBe(_disposable.Object);
		}

        [Test]
        public void ShouldAssignDisposableWhenSetter()
        {
            // Given
            var target = CreateTarget();

            // When   
            target.Disposable = _disposable.Object;

            // Then
            target.Disposable.ShouldBe(_disposable.Object);
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
            var target = CreateTarget(_disposable.Object);

            // When            
            target.Disposable = _disposable2.Object;

            // Then
            _disposable.Verify(i => i.Dispose());
            target.Disposable.ShouldBe(_disposable2.Object);
        }

        [Test]
        public void ShouldDisposeAfterSetWhenDisposed()
        {
            // Given
            var target = CreateTarget();

            // When   
            target.Dispose();
            target.Disposable = _disposable.Object;

            // Then
            _disposable.Verify(i => i.Dispose());
            target.Disposable.ShouldBe(_disposable.Object);
        }

        [Test]
        public void ShouldDisposWhenDisposed()
        {
            // Given
            var target = CreateTarget();

            // When   
            target.Disposable = _disposable.Object;
            target.Dispose();

            // Then
            _disposable.Verify(i => i.Dispose());
            target.Disposable.ShouldBe(_disposable.Object);
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
