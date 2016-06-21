namespace DevTeam.Patterns.Reactive.Tests
{
    using System;
    using System.Linq;

    using Castle.Components.DictionaryAdapter;

    using Dispose;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
	public class ObservableTests
	{		
		[Test]
		public void ShouldProvideEmptyObservable()
		{
			// Given
			
            // When
		    var events = Observable.Empty<int>().Materialize().ToList();

            // Then
            events.Count.ShouldBe(1);
            events[0].EventType.ShouldBe(Event<int>.Type.OnComplete);            
        }

        [Test]
        public void ShouldCreateObservable()
        {
            // Given

            // When
            var events = Observable.Create<int>(observer =>
                {
                    observer.OnNext(0);
                    observer.OnNext(1);
                    observer.OnNext(2);
                    observer.OnCompleted();
                    return Disposable.Empty();
                }).Materialize().ToList();

            // Then
            events.Count.ShouldBe(4);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(2);

            events[3].EventType.ShouldBe(Event<int>.Type.OnComplete);            
        }

        [Test]
        public void ShouldCreateObservableWhenEmpty()
        {
            // Given

            // When
            var events = Observable.Create<int>(observer =>
            {
                observer.OnCompleted();
                return Disposable.Empty();
            }).Materialize().ToList();

            // Then
            events.Count.ShouldBe(1);
            events[0].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldCreateObservableWhenError()
        {
            // Given
            var error = new Exception("");

            // When
            var events = Observable.Create<int>(observer =>
            {
                observer.OnNext(0);
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnError(error);
                return Disposable.Empty();
            }).Materialize().ToList();

            // Then
            events.Count.ShouldBe(4);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(2);

            events[3].EventType.ShouldBe(Event<int>.Type.OnError);
            events[3].Error.ShouldBe(error);
        }

        [Test]
        public void ShouldConvertToObservable()
        {
            // Given

            // When
            var events = 
                Observable.ToObservable(0, 1, 2)
                .Materialize()
                .ToList();

            // Then
            events.Count.ShouldBe(4);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(2);

            events[3].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldSelect()
        {
            // Given

            // When
            var events = Observable.ToObservable(0, 1, 2).Select(i => i.ToString()).Materialize().ToList();

            // Then
            events.Count.ShouldBe(4);

            events[0].EventType.ShouldBe(Event<string>.Type.OnNext);
            events[0].Value.ShouldBe("0");

            events[1].EventType.ShouldBe(Event<string>.Type.OnNext);
            events[1].Value.ShouldBe("1");

            events[2].EventType.ShouldBe(Event<string>.Type.OnNext);
            events[2].Value.ShouldBe("2");

            events[3].EventType.ShouldBe(Event<string>.Type.OnComplete);
        }

        [Test]
        public void ShouldConcat()
        {
            // Given

            // When
            var events = Observable.ToObservable(0, 1)
                .Concat(Observable.ToObservable(2))
                .Materialize()
                .ToList();

            // Then
            events.Count.ShouldBe(4);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(2);

            events[3].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldConcatWhenEmpty()
        {
            // Given

            // When
            var events = Observable.ToObservable(0, 1)
                .Concat(Observable.ToObservable<int>())
                .Concat(Observable.ToObservable(2))
                .Materialize()
                .ToList();

            // Then
            events.Count.ShouldBe(4);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(2);

            events[3].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldMaterializeAndDematerialize()
        {
            // Given

            // When
            var events = Observable.ToObservable(0, 1, 2)
                .Materialize()
                .Dematerialize()
                .Materialize()
                .ToList();

            // Then
            events.Count.ShouldBe(4);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(2);

            events[3].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldObserveOnScheduler()
        {
            // Given
            var scheduler = new ManualScheduler();
            var events = new EditableList<Event<int>>();

            // When
            Observable.ToObservable(0, 1, 2).ObserveOn(scheduler).MaterializeTo(events);                

            // Then
            events.Count.ShouldBe(0);

            scheduler.Process(1);
            events.Count.ShouldBe(1);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            scheduler.Process();
            events.Count.ShouldBe(4);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(2);

            events[3].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldSubscribeOnScheduler()
        {
            // Given
            var scheduler = new ManualScheduler();
            var events = new EditableList<Event<int>>();

            // When
            Observable.ToObservable(0, 1, 2).SubscribeOn(scheduler).MaterializeTo(events);

            // Then
            events.Count.ShouldBe(0);

            scheduler.Process(1);
            events.Count.ShouldBe(4);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(2);

            events[3].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }
    }
}
