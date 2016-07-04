namespace Echo
{
    using System;

    internal class ConsoleEchoPublisher: IEchoPublisher
    {
        public void OnNext(IEcho value)
        {
            Console.WriteLine(value.EchoMessage);
        }

        public void OnError(Exception error)
        {            
        }

        public void OnCompleted()
        {
            Console.WriteLine("Done.");
        }
    }
}
