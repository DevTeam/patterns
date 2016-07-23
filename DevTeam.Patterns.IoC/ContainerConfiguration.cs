namespace DevTeam.Patterns.IoC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class ContainerConfiguration: IConfiguration
    {
        internal static readonly IConfiguration Shared = new ContainerConfiguration();

        private ContainerConfiguration()
        {
        }

        public IEnumerable<IConfiguration> GetDependencies()
        {
            yield break;
        }

        public IEnumerable<IRegistration> CreateRegistrations(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            
            // Resolve All as IEnumerable
            yield return container
                .Using<IRegistrationComparer>(WellknownRegistrationComparer.AnyStateTypeAndKey)
               .Register(
                typeof(EmptyState),
                typeof(IEnumerable<>),
                ctx =>
                {                    
                    var enumItemType = ctx.ResolvingInstanceType.GenericTypeArguments[0];
                    var enumType = typeof(Enumerable<>).MakeGenericType(enumItemType);
                    var source =
                        from key in ctx.ResolvingContainer.Registrations
                        where key.InstanceType == enumItemType && key.StateType == ctx.Registration.StateType
                        select ctx.ResolvingContainer.Resolve(key.StateType, enumItemType, ctx.State, key.Key);
                    return Activator.CreateInstance(enumType, source);
                });
        }

        private class Enumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerable<object> _source;

            public Enumerable(IEnumerable<object> source)
            {
                if (source == null) throw new ArgumentNullException(nameof(source));

                _source = source;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return new Enumerator<T>(_source.GetEnumerator());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class Enumerator<T>: IEnumerator<T>
        {
            private readonly IEnumerator<object> _source;

            public Enumerator(IEnumerator<object> source)
            {
                if (source == null) throw new ArgumentNullException(nameof(source));

                _source = source;
            }

            public T Current => (T) _source.Current;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                return _source.MoveNext();
            }

            public void Reset()
            {
                _source.Reset();
            }

            public void Dispose()
            {
                _source.Dispose();
            }
        }
    }
}