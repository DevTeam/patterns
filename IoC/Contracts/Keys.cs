namespace IoC.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Keys
    {
        public static IKey Create(object key = null)
        {
            return new KeyImpl(Enumerable.Empty<IContractKey>(), Enumerable.Empty<IStateKey>(), key);
        }

        public static IKey Implementing(this IKey key, IEnumerable<IContractKey> contractKeys)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (contractKeys == null) throw new ArgumentNullException(nameof(contractKeys));

            return new KeyImpl(key.Contracts.Concat(contractKeys), key.States, key.Key);
        }

        public static IKey Implementing(this IKey key, params IContractKey[] contractKeys)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (contractKeys == null) throw new ArgumentNullException(nameof(contractKeys));

            return key.Implementing((IEnumerable<IContractKey>)contractKeys);
        }

        public static IKey Implementing(this IKey key, params Type[] contractTypes)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (contractTypes == null) throw new ArgumentNullException(nameof(contractTypes));

            return key.Implementing(contractTypes.Select(i => (IContractKey)new ContractKeyImpl(i)));
        }

        public static IKey Implementing<TContract>(this IKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return key.Implementing(typeof(TContract));
        }

        public static IKey Receiving(this IKey key, IEnumerable<IStateKey> stateKeys)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (stateKeys == null) throw new ArgumentNullException(nameof(stateKeys));

            return new KeyImpl(key.Contracts, key.States.Concat(stateKeys), key.Key);
        }

        public static IKey Receiving(this IKey key, params IStateKey[] stateKeys)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (stateKeys == null) throw new ArgumentNullException(nameof(stateKeys));

            return key.Receiving((IEnumerable<IStateKey>)stateKeys);
        }

        public static IKey Receiving(this IKey key, params Type[] stateTypes)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (stateTypes == null) throw new ArgumentNullException(nameof(stateTypes));

            return key.Receiving(stateTypes.Select(i => (IStateKey)new StateKeyImpl(i)));
        }

        public static IKey Receiving<TState>(this IKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            return key.Receiving(typeof(TState));
        }

        private struct StateKeyImpl : IStateKey
        {
            public StateKeyImpl(Type stateType)
            {
                StateType = stateType;
            }

            public Type StateType { get; }

            public override string ToString()
            {
                return $"{StateType.Name}";
            }
        }

        private struct ContractKeyImpl : IContractKey
        {
            public ContractKeyImpl(Type contractType)
            {
                if (contractType == null) throw new ArgumentNullException(nameof(contractType));

                ContractType = contractType;
            }

            public Type ContractType { get; }

            public override string ToString()
            {
                return $"{ContractType.Name}";
            }
        }

        private class KeyImpl : IKey
        {
            private readonly Lazy<IContractKey[]> _contracts;
            private readonly Lazy<IStateKey[]> _states;
            private readonly Lazy<int> _hashCode;

            public KeyImpl(IEnumerable<IContractKey> contracts, IEnumerable<IStateKey> states, object key)
            {
                if (contracts == null) throw new ArgumentNullException(nameof(contracts));
                if (states == null) throw new ArgumentNullException(nameof(states));
                if (key == null) throw new ArgumentNullException(nameof(key));

                _contracts = new Lazy<IContractKey[]>(() => contracts.OrderBy(i => i.ContractType).ToArray());
                _states = new Lazy<IStateKey[]>(() => states.OrderBy(i => i.StateType).ToArray());
                _hashCode = new Lazy<int>(GetHashCodeInternal);
                Key = key;
            }

            public IEnumerable<IContractKey> Contracts => _contracts.Value;

            public IEnumerable<IStateKey> States => _states.Value;

            public object Key { get; }

            public bool Equals(KeyImpl other)
            {
                return Equals(Key, other.Key) 
                    && Equals(i=> i.ContractType, Contracts, other.Contracts) 
                    && Equals(i => i.StateType, States, other.States);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is KeyImpl && Equals((KeyImpl)obj);
            }

            public override int GetHashCode()
            {
                return _hashCode.Value;
            }

            public override string ToString()
            {
                return $"Key [Contracts: {string.Join(",", Contracts.Select(i => i.ContractType.ToString()))}, States: {string.Join(",", States.Select(i => i.StateType.ToString()))}, Key: {Key ?? "null"}]";
            }

            private int GetHashCodeInternal()
            {
                unchecked
                {
                    var hashCode = GetHashCode(i => i.ContractType.GetHashCode(), Contracts);
                    hashCode = (hashCode * 397) ^ GetHashCode(i => i.StateType.GetHashCode(), States);
                    hashCode = (hashCode * 397) ^ (Key?.GetHashCode() ?? 0);
                    return hashCode;
                }
            }

            private static int GetHashCode<T>(Func<T, int> hashCodeFunc, IEnumerable<T> objects)
            {
                unchecked
                {
                   return objects.Aggregate(0, (hashCode, obj) => (hashCode * 397) ^ hashCodeFunc(obj));
                }
            }

            private static bool Equals<T>(Func<T, object> selector, IEnumerable<T> objects1, IEnumerable<T> objects2)
            {
                return objects1.Select(selector).SequenceEqual(objects2.Select(selector));
            }
        }
    }
}
