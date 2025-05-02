using Domain.Base;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Domain.Common.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; private set; }

        public PhoneNumber()
        {
            
        }
        private PhoneNumber(string phone)
        {
            Value = phone.ToLowerInvariant();
        }

        public static ResultDTO<PhoneNumber> Create(string phone)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(phone))
                errors.Add("Phone number is required.");
            else if (!Regex.IsMatch(phone, @"^09\d{9}$"))
                errors.Add("Invalid phone number format.");

            if (errors.Count > 0)
                return ResultDTO<PhoneNumber>.Failure("Invalid Phone Number", errors, "Please fix the phone number input.");

            return ResultDTO<PhoneNumber>.Success("Phone number created successfully", new PhoneNumber(phone), null);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }
    }
}
