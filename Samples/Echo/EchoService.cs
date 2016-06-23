namespace Echo
{
    using System;

    using DevTeam.Patterns.Dispose;
    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.Reactive;

    public class EchoService: IEchoService, IObserver<EchoRequest>, IObservable<Echo>
    {
        private readonly string _id;
        private readonly IEventAggregator _eventAggregator;
        private readonly Subject<Echo> _echoSubject = new Subject<Echo>();

        public EchoService(
            string id, 
            IEventAggregator eventAggregator)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));

            _id = id;
            _eventAggregator = eventAggregator;
        }

        public IDisposable Activate()
        {
            return new CompositeDisposable(
                _eventAggregator.RegisterProvider(this),
                _eventAggregator.RegisterConsumer(this));
        }

        IDisposable IObservable<Echo>.Subscribe(IObserver<Echo> observer)
        {
            return _echoSubject.Subscribe(observer);            
        }

        void IObserver<EchoRequest>.OnNext(EchoRequest value)
        {
            _echoSubject.OnNext(new Echo($"echo from {_id} \"{value.Message}\""));
        }

        void IObserver<EchoRequest>.OnError(Exception error)
        {
            _echoSubject.OnError(error);
        }

        void IObserver<EchoRequest>.OnCompleted()
        {
            _echoSubject.OnCompleted();
        }
    }
}
