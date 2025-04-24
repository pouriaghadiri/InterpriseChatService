using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Common.ValueObjects
{
    public class PhoneNumber : ValueObject
    {
        public string Value { get; private set; }

        private PhoneNumber() { }

        public PhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("phone number is required.");

            if (!Regex.IsMatch(phone, @"^09\d{9}$"))
                throw new ArgumentException("Invalid phone number format.");

            Value = phone.ToLowerInvariant();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLowerInvariant();
        }
    }
}
