namespace Echo
{
    using System;

    using DevTeam.Patterns.IoC;

    internal class Echo: IEcho
    {
        public Echo([State] string echoMessage)
        {
            if (echoMessage == null) throw new ArgumentNullException(nameof(echoMessage));

            EchoMessage = echoMessage;
        }

        public string EchoMessage { get; }
    }
}
