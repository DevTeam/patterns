namespace Echo
{
    using System;

    internal interface IEchoRequestSource : IObservable<EchoRequest>
    {
    }
}