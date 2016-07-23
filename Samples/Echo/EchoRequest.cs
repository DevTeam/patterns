namespace Echo
{
    using System;

    using DevTeam.Patterns.IoC;

    internal class EchoRequest : IEchoRequest
    {
        public EchoRequest([State] string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Message = message;
        }

        public string Message { get; }
    }
}
