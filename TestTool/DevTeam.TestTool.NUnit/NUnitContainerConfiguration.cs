namespace DevTeam.TestTool.NUnit
{
    using System;
    using System.Collections.Generic;

    using Engine.Contracts;

    using Patterns.IoC;
    using Patterns.Reactive;

    using Platform.Reflection;

    /// <inheritdoc/>
    public class NUnitContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield return new ReactiveContainerConfiguration();
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Using<ILifetime>(WellknownLifetime.Singleton).Register<ISession, ITestSource>(session => new TestSource(session, container.Resolve<IReflection>(), container.Resolve<IProperty>(WellknownProperty.Assembly)), "nunit");
        }
    }
}
