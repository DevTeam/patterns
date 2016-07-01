namespace DevTeam.Patterns.Reactive.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Castle.Components.DictionaryAdapter;

    using Dispose;

    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
	public class ObservableTests
	{
        [Test]
        public void ShouldDoToEnumerable()
        {
            // Given

            // When
            var events = Observable.ToObservable(0, 1, 2)
                .ToEnumerable()
                .ToList();

            // Then
            events.SequenceEqual(new[] { 0, 1, 2 }).ShouldBeTrue();
        }

        [Test]
        public void ShouldDoToEnumerableWhenError()
        {
            // Given
            var expectedException = new Exception();
            Exception actualException = null;

            // When
            try
            {
                var events = Observable.ToObservable(0, 1, 2)
                .Concat(expectedException.AsObservable<int>())
                .ToEnumerable()
                .ToList();
            }
            catch (Exception ex)
            {
                actualException = ex;
            }

            // Then
            actualException.ShouldBe(expectedException);
        }

        [Test]
        public void ShouldDoToEnumerableWhenEmpty()
        {
            // Given

            // When
            var events = Observable.Empty<int>()
                .ToEnumerable()
                .ToList();

            // Then
            events.ShouldBeEmpty();
        }

        [Test]
		public void ShouldProvideEmptyObservable()
		{
			// Given
			
            // When
		    var events = Observable.Empty<int>()
                .Materialize()
                .ToEnumerable()
                .ToList();

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
                })
                .Materialize()
                .ToEnumerable()
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
        public void ShouldCreateObservableWhenEmpty()
        {
            // Given

            // When
            var events = Observable.Create<int>(observer =>
            {
                observer.OnCompleted();
                return Disposable.Empty();
            }).Materialize()
            .ToEnumerable()
            .ToList();

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
            }).Materialize()
            .ToEnumerable()
            .ToList();

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
                .ToEnumerable()
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
            var events = 
                Observable.ToObservable(0, 1, 2)
                .Select(i => i.ToString())
                .Materialize()
                .ToEnumerable()
                .ToList();

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
        public void ShouldSelectMany()
        {
            // Given

            // When
            var events = 
                Observable.ToObservable(
                    Observable.ToObservable(0), 
                    Observable.ToObservable(0, 1), 
                    Observable.ToObservable(0, 1, 2))
                .SelectMany(i => i)
                .Materialize()
                .ToEnumerable()
                .ToList();

            // Then
            events.Count.ShouldBe(7);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(0);

            events[2].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[2].Value.ShouldBe(1);

            events[3].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[3].Value.ShouldBe(0);

            events[4].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[4].Value.ShouldBe(1);

            events[5].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[5].Value.ShouldBe(2);

            events[6].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldConcat()
        {
            // Given

            // When
            var events = Observable.ToObservable(0, 1)
                .Concat(Observable.ToObservable(2))
                .Materialize()
                .ToEnumerable()
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
                .ToEnumerable()
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
        public void ShouldMaterialize()
        {
            // Given

            // When
            var events = Observable.ToObservable(0, 1, 2)
                .Materialize()
                .ToEnumerable()
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
        public void ShouldMaterializeWhenEmpty()
        {
            // Given

            // When
            var events = Observable.Empty<int>()
                .Materialize()
                .ToEnumerable()
                .ToList();

            // Then
            events.Count.ShouldBe(1);

            events[0].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldMaterializeWhenError()
        {
            // Given
            var ex = new Exception();

            // When
            var events = ex.AsObservable<int>()
                .Materialize()
                .ToEnumerable()
                .ToList();

            // Then
            events.Count.ShouldBe(1);

            events[0].EventType.ShouldBe(Event<int>.Type.OnError);
            events[0].Error.ShouldBe(ex);
        }

        [Test]
        public void ShouldMaterializeAndDematerialize()
        {
            // Given

            // When
            var items = Observable.ToObservable(0, 1, 2)
                .Materialize()
                .Dematerialize()
                .ToEnumerable()
                .ToList();

            // Then
            items.Count.ShouldBe(3);

            items.SequenceEqual(new[] { 0, 1, 2 }).ShouldBeTrue();
        }

        [Test]
        public void ShouldMaterializeAndDematerializeWhenEmpty()
        {
            // Given

            // When
            var items = Observable.Empty<int>()
                .Materialize()
                .Dematerialize()
                .ToEnumerable()
                .ToList();

            // Then
            items.ShouldBeEmpty();
        }

        [Test]
        public void ShouldObserveOnScheduler()
        {
            // Given
            var scheduler = new ManualScheduler();
            var events = new EditableList<Event<int>>();

            // When
            Observable.ToObservable(0, 1, 2)
                .ObserveOn(scheduler)
                .Materialize()
                .Subscribe(i => events.Add(i), e => { }, () => { });

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
            Observable.ToObservable(0, 1, 2)
                .SubscribeOn(scheduler)
                .Materialize()
                .Subscribe(i => events.Add(i), e => { }, () => { });

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

        [Test]
        public void ShouldObserveTaskResult()
        {
            // Given         
            var task = Task.Run(() => 1);

            // When
            var events = task
                .ToObservable()
                .Materialize()
                .ToEnumerable()
                .ToList();

            // Then
            events.Count.ShouldBe(2);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(1);

            events[1].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }

        [Test]
        public void ShouldObserveTaskFault()
        {
            // Given            
            var ex = new Exception();
            var task = Task.Run((Func<int>)(() => { throw ex; }));

            // When
            var events = task
                .ToObservable()
                .Materialize()
                .ToEnumerable()
                .ToList();

            // Then
            events.Count.ShouldBe(1);

            events[0].EventType.ShouldBe(Event<int>.Type.OnError);
            events[0].Error.ShouldBe(ex);            
        }

        [Test]
        public void ShouldObserveTasks()
        {
            // Given     
            var tasks = new[] { Task.Run(() => 0), Task.Run(() => 1) };

            // When
            var events = tasks
                .ToObservable()
                .Materialize()
                .ToEnumerable()
                .ToList();

            // Then
            events.Count.ShouldBe(3);

            events[0].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[0].Value.ShouldBe(0);

            events[1].EventType.ShouldBe(Event<int>.Type.OnNext);
            events[1].Value.ShouldBe(1);

            events[2].EventType.ShouldBe(Event<int>.Type.OnComplete);
        }
    }
}
