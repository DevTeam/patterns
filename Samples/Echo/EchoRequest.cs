namespace Echo
{
    using System;

    internal class EchoRequest
    {
        public EchoRequest(string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Message = message;
        }

        public string Message { get; }
    }
}
