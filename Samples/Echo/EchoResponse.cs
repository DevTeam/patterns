namespace Echo
{
    using System;

    internal class EchoResponse
    {
        public EchoResponse(string echo)
        {
            if (echo == null) throw new ArgumentNullException(nameof(echo));

            Echo = echo;
        }

        public string Echo { get; }
    }
}
