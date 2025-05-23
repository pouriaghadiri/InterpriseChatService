using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Common.ValueObjects
{
    public class Email : ValueObject
    {
        public string Value { get; private set; }
         
        private Email(string email)
        { 
            Value = email.ToLowerInvariant();
        }
        public static ResultDTO<Email> Create(string email)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email is required.");
            else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Invalid email format.");
            if (errors.Count > 0)
                return ResultDTO<Email>.Failure("Invalid Email", errors, "Please fix the email input.");
            return ResultDTO<Email>.Success("Valid Email", new Email(email), "Email created successfully");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }
    }
}
