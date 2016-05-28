namespace DevTeam.Patterns.Dispose.Tests
{
	using NUnit.Framework;

	using Shouldly;

	[TestFixture]
	public class DisposableTests
	{
		[Test]
		public void EmptyDisposableShouldBeDisposable()
		{
			// Given
			var target = Disposable.Empty();
			
			// When
			target.Dispose();
			
			// Then
		}

		[Test]
		public void EmptyDisposableShouldBeDisposableMoreOneOneTime()
		{
			// Given
			var target = Disposable.Empty();

			// When
			target.Dispose();
			target.Dispose();

			// Then
		}

		[Test]
		public void ShouldCreateDisposableFromAction()
		{
			// Given
			var disposed = false;
			var target = Disposable.Create(() => disposed = true);

			// When
			target.Dispose();

			// Then
			disposed.ShouldBe(true);
		}

		[Test]
		public void ShouldRunDisposeActionManyTimes()
		{
			// Given
			var disposed = 0;
			var target = Disposable.Create(() => disposed++);

			// When
			target.Dispose();
			target.Dispose();
			target.Dispose();

			// Then
			disposed.ShouldBe(3);
		}
	}
}
