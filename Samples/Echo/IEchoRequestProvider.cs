namespace Echo
{
    using System;

    internal interface IEchoRequestProvider : IObservable<EchoRequest>
    {
    }
}