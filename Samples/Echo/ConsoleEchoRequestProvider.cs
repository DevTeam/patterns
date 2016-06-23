namespace Echo
{
    using System;

    using DevTeam.Patterns.Dispose;

    internal class ConsoleEchoRequestProvider: IEchoRequestProvider
    {
        public IDisposable Subscribe(IObserver<EchoRequest> observer)
        {
            string input;
            do
            {
                input = Console.ReadLine();
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
