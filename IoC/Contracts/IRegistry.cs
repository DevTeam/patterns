namespace IoC.Contracts
{
    using System;

    public interface IRegistry
    {
        bool TryRegister(out IDisposable registrationToken, IKey key, IFactory factory, params IExtension[] extensions);
    }
}
