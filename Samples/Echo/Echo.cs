namespace Echo
{
    using System;

    internal class Echo: IEcho
    {
        public Echo(string echoMessage)
        {
            if (echoMessage == null) throw new ArgumentNullException(nameof(echoMessage));

            EchoMessage = echoMessage;
        }

        public string EchoMessage { get; }
    }
}
