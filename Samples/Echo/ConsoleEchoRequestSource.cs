namespace Echo
{
    using System;

    using DevTeam.Patterns.Dispose;
    using DevTeam.Patterns.IoC;

    internal class ConsoleEchoRequestSource: IEchoRequestSource
    {
        private readonly IResolver<string, IEchoRequest> _requestResolver;

        public ConsoleEchoRequestSource(
            IResolver<string, IEchoRequest> requestResolver)
        {
            if (requestResolver == null) throw new ArgumentNullException(nameof(requestResolver));

            _requestResolver = requestResolver;
        }

        public IDisposable Subscribe(IObserver<IEchoRequest> observer)
        {
            do
            {
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    observer.OnNext(_requestResolver.Resolve(input));                    
                }
                else
                {
                    break;
                }
            }
            while (true);

            observer.OnCompleted();
            return Disposable.Empty();
        }
    }
}
