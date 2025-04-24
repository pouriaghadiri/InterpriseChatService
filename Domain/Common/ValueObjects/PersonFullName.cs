using Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Common.ValueObjects
{
    public class PersonFullName:ValueObject
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public PersonFullName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name is required.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name is required.");
            FirstName = firstName;
            LastName = lastName;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return FirstName.ToLowerInvariant();
            yield return LastName.ToLowerInvariant();
        }
    }
}
