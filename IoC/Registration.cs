namespace IoC
{
    using System;
    using System.Collections.Generic;

    using Contracts;

    public class Registration: IRegistration, IExtension
    {
        public Registration(IContainer container, IKey key, IEnumerable<IExtension> extensions, IFactory factory)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (extensions == null) throw new ArgumentNullException(nameof(extensions));

            Container = container;
            Key = key;
            Extensions = extensions;
            Factory = factory;
        }

        public IContainer Container { get; }

        public IKey Key { get; }

        public IEnumerable<IExtension> Extensions { get; }

        internal IFactory Factory { get; }
    }
}
