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
        private MessageContent(string value)
        { 
            Value = value;
        }
        public static ResultDTO<MessageContent> Create(string value)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(value))
                errors.Add("Message content is required.");
            else if (value.Length > 1500)
                errors.Add("Message Content is too long");
            if (errors.Count > 0)
                return ResultDTO<MessageContent>.Failure("Invalid Message", errors, "Please fix the message input.");
            return ResultDTO<MessageContent>.Success("Valid Message", new MessageContent(value), "Message created successfully");
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
