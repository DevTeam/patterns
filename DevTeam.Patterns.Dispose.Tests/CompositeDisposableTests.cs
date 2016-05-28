namespace DevTeam.Patterns.Dispose.Tests
{
	using System;

	using Moq;

	using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class CompositeDisposableTests
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
		public void ShouldAddDisposable()
		{
			// Given
			var target = CreateTarget();
			
			// When
			target.Add(_disposable.Object);
			
			// Then
			target.Count.ShouldBe(1);
		}

		[Test]
		public void ShouldNotAddDuplicates()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Add(_disposable.Object);
			target.Add(_disposable.Object);
			target.Add(_disposable.Object);

			// Then
			target.Count.ShouldBe(1);
		}

		[Test]
		public void ShouldNotAddWhenAddingToDisposes()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Dispose();
			target.Add(_disposable.Object);

			// Then
			target.Count.ShouldBe(0);
		}

		[Test]
		public void ShouldDisposeObjectWhenAddingToDisposes()
		{
			// Given
			var target = CreateTarget();

			// When
			target.Dispose();
			target.Add(_disposable.Object);
		
			// Then
			_disposable.Verify(i => i.Dispose(), Times.Once);
		}

		[Test]
		public void ShouldAddDisposableViaCtor()
		{
			// Given

			// When
			var target = CreateTarget(_disposable.Object, _disposable2.Object);

			// Then
			target.Count.ShouldBe(2);
		}

		[Test]
		public void ShouldNotAddDuplicatesViaCtor()
		{
			// Given

			// When
			var target = CreateTarget(_disposable.Object, _disposable.Object, _disposable.Object, _disposable2.Object);

			// Then
			target.Count.ShouldBe(2);
		}

		[Test]
		public void ShouldRemoveDisposable()
		{
			// Given
			var target = CreateTarget();
			target.Add(_disposable.Object);

			// When
			target.Remove(_disposable.Object);

			// Then
			target.Count.ShouldBe(0);
		}

		[Test]
		public void ShouldDisposeObjectWhenRemove()
		{
			// Given
			var target = CreateTarget();
			target.Add(_disposable.Object);

			// When
			target.Remove(_disposable.Object);

			// Then
			_disposable.Verify(i => i.Dispose(), Times.Once);
		}

		[Test]
		public void ShouldClear()
		{
			// Given
			var target = CreateTarget();
			target.Add(_disposable.Object);
			target.Add(_disposable2.Object);

			// When
			target.Clear();

			// Then
			target.Count.ShouldBe(0);
		}

		[Test]
		public void ShouldClearWhenDisposeObject()
		{
			// Given
			var target = CreateTarget();
			target.Add(_disposable.Object);
			target.Add(_disposable2.Object);

			// When
			target.Dispose();

			// Then
			target.Count.ShouldBe(0);
		}

		[Test]
		public void ShouldDisposeAllObjectsWhenClear()
		{
			// Given
			var target = CreateTarget();
			target.Add(_disposable.Object);
			target.Add(_disposable2.Object);

			// When
			target.Clear();

			// Then
			_disposable.Verify(i => i.Dispose(), Times.Once);
			_disposable2.Verify(i => i.Dispose(), Times.Once);
		}

		[Test]
		public void ShouldDisposeAllObjectsWhenDispose()
		{
			// Given
			var target = CreateTarget();
			target.Add(_disposable.Object);
			target.Add(_disposable2.Object);

			// When
			target.Dispose();

			// Then
			_disposable.Verify(i => i.Dispose(), Times.Once);
			_disposable2.Verify(i => i.Dispose(), Times.Once);
		}

		private static CompositeDisposable CreateTarget(params IDisposable[] disposables)
		{
			return new CompositeDisposable(disposables);
		}
	}
}
