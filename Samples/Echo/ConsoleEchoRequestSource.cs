namespace Echo
{
    using System;

    using DevTeam.Patterns.Dispose;

    internal class ConsoleEchoRequestSource: IEchoRequestSource
    {
        public IDisposable Subscribe(IObserver<EchoRequest> observer)
        {
            do
            {
                var input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    observer.OnNext(new EchoRequest(input));                    
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
