using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.ValueObjects
{
    public class EntityName : ValueObject
    {
        public string Value { get; private set; }
        public EntityName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name content is required.");
            if (value.Length > 100)
                throw new ArgumentException("Name Content is too long");

            Value = value;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
