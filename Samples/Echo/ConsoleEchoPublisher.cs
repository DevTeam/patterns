namespace Echo
{
    using System;

    internal class ConsoleEchoPublisher: IEchoPublisher
    {
        public void OnNext(EchoResponse value)
        {
            Console.WriteLine(value.Echo);
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
