using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.ValueObjects
{
    public class MessageContent : ValueObject
    {
        public string Value { get; private set; }
        private MessageContent() { }
        public MessageContent(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Message content is required.");
            if (value.Length > 1500)
                throw new ArgumentException("Message Content is too long");

            Value = value;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
