namespace DevTeam.TestTool.Engine.Host
{
    using System;
    using System.Text.RegularExpressions;

    using Contracts;

    internal class BaseProperty : IProperty
    {
        private readonly Regex _namePattern;
        private readonly Regex _valuePattern;

        protected BaseProperty(string id, string description, string namePatternStr, string valuePatternStr)
            :this(id, description, new Regex(namePatternStr, RegexOptions.Singleline | RegexOptions.CultureInvariant), new Regex(valuePatternStr, RegexOptions.Singleline | RegexOptions.CultureInvariant))
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (namePatternStr == null) throw new ArgumentNullException(nameof(namePatternStr));
            if (valuePatternStr == null) throw new ArgumentNullException(nameof(valuePatternStr));
        }

        private BaseProperty(string id, string description, Regex namePattern, Regex valuePattern)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (namePattern == null) throw new ArgumentNullException(nameof(namePattern));
            if (valuePattern == null) throw new ArgumentNullException(nameof(valuePattern));

            _namePattern = namePattern;
            _valuePattern = valuePattern;
            Id = id;
            Description = description;            
        }

        public string Id { get; }

        public string Description { get; }

        public bool Match(string name)
        {
            return _namePattern.Match(name).Success;
        }

        public bool Validate(string value)
        {
            return _valuePattern.Match(value).Success;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BaseProperty)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private bool Equals(IProperty other)
        {
            return string.Equals(Id, other.Id);
        }
    }
}
