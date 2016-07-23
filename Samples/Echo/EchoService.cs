namespace Echo
{
    using System;

    using DevTeam.Patterns.Dispose;
    using DevTeam.Patterns.EventAggregator;
    using DevTeam.Patterns.IoC;
    using DevTeam.Patterns.Reactive;

    internal class EchoService: IEchoService, IObserver<IEchoRequest>, IObservable<IEcho>
    {
        private readonly string _id;
        private readonly IEventAggregator _eventAggregator;
        private readonly IResolver<string, IEcho> _echoResolver;
        private readonly ISubject<IEcho> _echoSubject;

        public EchoService(
            [State] string id, 
            [Dependency] IEventAggregator eventAggregator,
            [Dependency] IResolver<string, IEcho> echoResolver,
            [Dependency(Key = WellknownSubject.Simple)] ISubject<IEcho> echoSubject)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (eventAggregator == null) throw new ArgumentNullException(nameof(eventAggregator));
            if (echoResolver == null) throw new ArgumentNullException(nameof(echoResolver));
            if (echoSubject == null) throw new ArgumentNullException(nameof(echoSubject));

            _echoSubject = echoSubject;
            _id = id;
            _eventAggregator = eventAggregator;
            _echoResolver = echoResolver;
        }

        public IDisposable Activate()
        {
            return new CompositeDisposable(
                _eventAggregator.RegisterProvider(this),
                _eventAggregator.RegisterConsumer(this));
        }

        IDisposable IObservable<IEcho>.Subscribe(IObserver<IEcho> observer)
        {
            return _echoSubject.Subscribe(observer);            
        }

        void IObserver<IEchoRequest>.OnNext(IEchoRequest value)
        {
            _echoSubject.OnNext(_echoResolver.Resolve($"echo from {_id} \"{value.Message}\""));
        }

        void IObserver<IEchoRequest>.OnError(Exception error)
        {
            _echoSubject.OnError(error);
        }

        void IObserver<IEchoRequest>.OnCompleted()
        {
            _echoSubject.OnCompleted();
        }
    }
}
