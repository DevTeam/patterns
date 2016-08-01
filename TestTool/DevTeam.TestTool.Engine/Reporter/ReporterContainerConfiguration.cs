namespace DevTeam.TestTool.Engine.Reporter
{
    using System;
    using System.Collections.Generic;

    using Patterns.IoC;
    using Contracts;

    /// <inheritdoc/>
    internal class ReporterContainerConfiguration: IConfiguration
    {
        /// <inheritdoc/>
        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            yield return container.Register<ReporterTool>().As<ISession, ITool>();
            yield return container.Register<TextTestReporter>(WellknownLifetime.Singleton).As<ITestReporter>("text");
        }
    }
}
