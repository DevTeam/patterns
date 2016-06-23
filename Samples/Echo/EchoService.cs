namespace Echo
{
    using System;

    using DevTeam.Patterns.Dispose;
    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.Reactive;

    public class EchoService: IEchoService, IObserver<EchoRequest>, IObservable<EchoResponse>
    {
        private readonly string _id;
        private readonly IEventAggregator _eventAggregator;
        private readonly Subject<EchoResponse> _echoSubject = new Subject<EchoResponse>();

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

        IDisposable IObservable<EchoResponse>.Subscribe(IObserver<EchoResponse> observer)
        {
            return _echoSubject.Subscribe(observer);
        }

        void IObserver<EchoRequest>.OnNext(EchoRequest value)
        {
            _echoSubject.OnNext(new EchoResponse($"echo from {_id} \"{value.Message}\""));
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
